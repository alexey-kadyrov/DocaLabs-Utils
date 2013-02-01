using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DocaLabs.Http.Client.Serialization
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
        public static TResult Parse<TResult>(this WebRequest request)
        {
            using (var response = new HttpResponse(request.GetResponse()))
            {
                return TransformResult<TResult>(response);
            }
        }

        static TResult TransformResult<TResult>(HttpResponse response)
        {
            var attrs = typeof(TResult).GetCustomAttributes(typeof(ResponseDeserializationAttribute), true);
            if (attrs.Length > 0)
                return ((IResponseDeserialization)attrs[0]).Deserialize<TResult>(response);

            if (typeof (TResult) == typeof (VoidType))
                return default(TResult);

            var provider = FindProvider<TResult>(response);
            if (provider != null)
                return provider.Deserialize<TResult>(response);

            throw new UnrecoverableHttpClientException(Resources.Text.cannot_figure_out_how_to_deserialize);
        }

        static IResponseDeserialization FindProvider<TResult>(HttpResponse response)
        {
            List<IResponseDeserializationProvider> providers;

            lock (Locker)
            {
                providers = _providers;
            }

            return providers.FirstOrDefault(x => x.CheckIfSupports<TResult>(response));
        }

        /// <summary>
        /// Returns true if the type is primitive or string/decimal/Guid/dateTime/TimeSpan/DateTimeOffset.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimpleType(this Type type)
        {
            return (type == typeof(string) ||
                    type.IsPrimitive ||
                    type == typeof(decimal) ||
                    type == typeof(Guid) ||
                    type == typeof(DateTime) ||
                    type == typeof(TimeSpan) ||
                    type == typeof(DateTimeOffset) ||
                    type.IsEnum);
        }
    }
}
