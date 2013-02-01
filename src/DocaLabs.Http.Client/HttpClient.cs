using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using DocaLabs.Http.Client.Configuration;
using DocaLabs.Http.Client.ContentEncoding;
using DocaLabs.Http.Client.Deserialization;
using DocaLabs.Http.Client.Mapping;
using DocaLabs.Http.Client.Serialization;

namespace DocaLabs.Http.Client
{
    /// <summary>
    /// Base class for strong typed support for RESTFull service clients.
    /// The concept is that each service endpoint (the service Url) and each protocol method (such as GET, PUT, POST)
    /// is considered to be a separate service. 
    /// For testability it's advisable to define an interface for each service definition.
    /// Like (which should sufficient for most cases):
    /// public interface IMyService
    /// {
    ///     MyData Execute(MyQuery query);
    /// }
    /// public class MyService : HttpClient&lt;MyQuery, MyData>, IMyService
    /// { // the Url must be placed into the AppSettings using the full class name as the key.
    /// }
    /// or
    /// public class MyService : HttpClient&lt;MyQuery, MyData>, IMyService
    /// {
    ///     public MyService() : base(new Uri("http://foo.com/"))
    ///     {
    ///     }
    /// }
    /// </summary>
    /// <typeparam name="TQuery">Type which will be used as input parameters that can be serialized into query string or the request stream.</typeparam>
    /// <typeparam name="TResult">Type which will be used as output data that will be deserialized from the response stream.</typeparam>
    public class HttpClient<TQuery, TResult>
    {
        /// <summary>
        /// Gets a service Url
        /// </summary>
        public Uri ServiceUrl { get; private set; }

        /// <summary>
        /// Gets a protocol method used in the request.
        /// If the method is not defined explicitly (returns non null value) then if there is RequestSerializationAttribute 
        /// defined either on the TQuery class or one of its properties or on the HttpVlient's subclass then the method will be POST
        /// otherwise it's GET.
        /// </summary>
        public virtual string RequestMethod { get { return null; } }

        /// <summary>
        /// Gets or sets the request timeout, the default value is 90 seconds.
        /// </summary>
        public int RequestTimeout { get; set; }

        /// <summary>
        /// Get or sets whenever to add 'Accept-Encoding' header automatically depending on what content decoders are defined in ContentDecoderFactory.
        /// The default value is true.
        /// </summary>
        public bool AutoSetAcceptEncoding { get; set; }

        /// <summary>
        /// Gets the service configuration if it's defined in the config file, otherwise null.
        /// </summary>
        protected HttpClientEndpointElement Configuration { get; private set; }

        /// <summary>
        /// Retry strategy for calling the remote endpoint.
        /// </summary>
        protected Func<Func<TResult>, TResult> RetryStrategy { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HttpClient.
        /// 
        /// </summary>
        /// <param name="serviceUrl">The URL of the service.</param>
        /// <param name="configurationName">If the configuration name is not null it'll be used to get the endpoint configuration from the config file.</param>
        /// <param name="retryStrategy">If the parameter null then the default retry strategy will be used.</param>
        public HttpClient(Uri serviceUrl = null, string configurationName = null, Func<Func<TResult>, TResult> retryStrategy = null)
        {
            ServiceUrl = serviceUrl;
            RetryStrategy = retryStrategy ?? GetDefaultRetryStrategy();
            AutoSetAcceptEncoding = true;

            ReadConfiguration(configurationName);

            if(ServiceUrl == null)
                throw new ArgumentException(Resources.Text.service_url_is_not_defined, "serviceUrl");
        }

        /// <summary>
        /// Executes a http request. By default all properties with public getters are serialized into the http query part.
        /// The input data serialization behaviour can be altered by:
        ///   * Using QueryIgnoreAttribute (on class or property level),
        ///   * Using one of the RequestSerializationAttribute derived classes (on the class or property level) 
        ///   * Implementing ICustomQueryMapper interface for custom mapping to query string
        ///   * Overriding TryMakeQueryString and/or TryWriteRequestData
        /// The output data deserialization behaviour can be altered by:
        ///   * Using one of the ResponseDeserializationAttribute derived classes (on the class level)
        ///   * Overriding ParseResponse
        /// </summary>
        /// <param name="query">Input parameters.</param>
        /// <returns>Output data.</returns>
        public TResult Execute(TQuery query)
        {
            try
            {
                return RetryStrategy(() => DoExecute(query));
            }
            catch(Exception e)
            {
                if (e is HttpClientException)
                    throw;

                throw new HttpClientException(string.Format(Resources.Text.failed_execute_request, ServiceUrl, GetType().FullName), e);
            }
        }

        protected virtual TResult DoExecute(TQuery query)
        {
            var request = CreateRequest(UrlBuilder.CreateUrl(ServiceUrl, query));

            TryWriteRequestData(query, request);

            return ParseResponse(query, request);
        }

        protected virtual WebRequest CreateRequest(Uri url)
        {
            var request = WebRequest.Create(url);

            request.Timeout = RequestTimeout;
            request.Method = GetRequestMethod();

            if (AutoSetAcceptEncoding)
                ContentDecoderFactory.AddAcceptEncodings(request);

            Configuration.AddHeaders(request);

            Configuration.AddClientCertificates(request as HttpWebRequest);

            Configuration.SetWebProxy(request);

            return request;
        }

        protected virtual string GetRequestMethod()
        {
            if (!string.IsNullOrWhiteSpace(RequestMethod))
                return RequestMethod;

            var type = typeof(TQuery);

            return type.GetCustomAttributes(typeof(RequestSerializationAttribute), true).Length > 0
                || GetType().GetCustomAttributes(typeof(RequestSerializationAttribute), true).Length > 0
                || type.GetProperties().Any(property => property.GetCustomAttributes(typeof(RequestSerializationAttribute), true).Length > 0)
                ? WebRequestMethods.Http.Post
                : WebRequestMethods.Http.Get;
        }

        protected virtual void TryWriteRequestData(TQuery query, WebRequest request)
        {
            var serializer = RequestSerializationFactory.GetSerializer(this, query);
            if(serializer != null)
                serializer.Serialize(query, request);
        }

        protected virtual TResult ParseResponse(TQuery query, WebRequest request)
        {
            return (TResult)request.Parse(typeof(TResult));
        }

        protected TResult DefaultRetryStrategy(Func<TResult> action, int times, int initialTimeout, int stepbackIncrease)
        {
            var timeout = initialTimeout;

            var attempt = 1;

            while (true)
            {
                try
                {
                    return action();
                }
                catch (UnrecoverableHttpClientException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    attempt++;

                    if (attempt >= times)
                        throw;

                    OnLogRetry(attempt, times, e);

                    Thread.Sleep(timeout);

                    timeout += stepbackIncrease;
                }
            }
        }
    
        protected virtual void OnLogRetry(int attempt, int times, Exception e)
        {
            if(e == null)
                Debug.Write(string.Format(Resources.Text.will_try_again, attempt, times));
            else
                Debug.WriteLine("{0} : {1}", string.Format(Resources.Text.will_try_again, attempt, times), e);
        }

        protected Func<Func<TResult>, TResult> GetDefaultRetryStrategy()
        {
            return action => DefaultRetryStrategy(action, 4, 1000, 1000);
        }
       
        void ReadConfiguration(string configurationName)
        {
            Configuration = GetConfigurationElement(configurationName);

            if (Configuration.ServiceUrl != null)
                ServiceUrl = Configuration.ServiceUrl;

            RequestTimeout = Configuration.Timeout;
        }

        HttpClientEndpointElement GetConfigurationElement(string configurationName)
        {
            if (string.IsNullOrWhiteSpace(configurationName))
                configurationName = GetType().FullName;

            var section = HttpClientEndpointSection.GetDefaultSection();

            return section != null
                ? section.Endpoints[configurationName] ?? new HttpClientEndpointElement()
                : new HttpClientEndpointElement();
        }
    }
}
