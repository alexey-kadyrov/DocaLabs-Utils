using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DocaLabs.Http.Client.Deserialization
{
    /// <summary>
    /// Defines helper methods to deserialize the web response.
    /// All public methods are thread safe.
    /// </summary>
    public static class ResponseParser
    {
        static readonly object Locker;
        static List<IResponseDeserializationProvider> _providers;

        /// <summary>
        /// Gets or sets the list of providers.
        /// </summary>
        public static List<IResponseDeserializationProvider> Providers
        {
            get
            {
                List<IResponseDeserializationProvider> providers;

                lock (Locker)
                {
                    providers = _providers;
                }

                return providers.ToList();
            }

            set
            {
                var providers = value.ToList();

                lock (Locker)
                {
                    _providers = providers;
                }
            }
        }

        static ResponseParser()
        {
            Locker = new object();

            _providers = new List<IResponseDeserializationProvider>
            {
                new JsonResponseDeserializer(),
                new XmlResponseDeserializer(),
                new PlainTextResponseDeserializer(),
                new ImageResponseDeserializer()
            };
        }

        /// <summary>
        /// Gets the web response and tries to deserialize the response. It tries to do:
        ///     1. To get ResponseDeserializationAttribute if defined on TResult class.
        ///     2. Tries to find deserialization provider among the registered.
        /// </summary>
        public static object Parse(WebRequest request, Type resultType)
        {
            if(request == null)
                throw new ArgumentNullException("request");

            using (var response = new HttpResponse(request))
            {
                return TransformResult(response, resultType);
            }
        }

        static object TransformResult(HttpResponse response, Type resultType)
        {
            if(resultType == null)
                throw new ArgumentNullException("resultType");

            var attrs = resultType.GetCustomAttributes(typeof(ResponseDeserializationAttribute), true);
            if (attrs.Length > 0)
                return ((IResponseDeserialization)attrs[0]).Deserialize(response, resultType);

            if (resultType == typeof (VoidType))
                return VoidType.Value;

            var provider = FindProvider(response, resultType);
            if (provider != null)
                return provider.Deserialize(response, resultType);

            throw new UnrecoverableHttpClientException(Resources.Text.cannot_figure_out_how_to_deserialize);
        }

        static IResponseDeserialization FindProvider(HttpResponse response, Type resultType)
        {
            List<IResponseDeserializationProvider> providers;

            lock (Locker)
            {
                providers = _providers;
            }

            return providers.FirstOrDefault(x => x.CheckIfSupports(response, resultType));
        }
    }
}
