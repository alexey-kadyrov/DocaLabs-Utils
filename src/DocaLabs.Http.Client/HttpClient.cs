using System;
using System.Diagnostics;
using System.Drawing;
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
    /// The concept is that each service endpoint (the service Url) and each protocol method (such as GET, PUT, POST) is considered to be a separate service. 
    /// For testability it's advisable to define an interface for each service definition and you can use HttpClientFactory.CreateInstance
    /// to create instance of a concrete class implementing the interface without manually defining it.
    /// 
    /// public interface IGoldenUserService
    /// {
    ///     User GetGoldenUser(GetUserQuery query);
    /// }
    /// 
    /// var userService = HttpClientFactory.CreateInstance&lt;IGoldenUserService>(); // the base URL must be defined in the app.config 
    ///     or
    /// var userService = HttpClientFactory.CreateInstance&lt;IGoldenUserService>("http://foo.com/");
    /// 
    /// var user = userService.GetGoldenUser(new GetUserQuery(userId));
    /// 
    /// </summary>
    /// <typeparam name="TQuery">Type which will be used as input parameters that can be serialized into query string or the request stream.</typeparam>
    /// <typeparam name="TResult">Type which will be used as output data that will be deserialized from the response stream.</typeparam>
    public class HttpClient<TQuery, TResult>
    {
        /// <summary>
        /// Gets a service Url
        /// </summary>
        public Uri BaseUrl { get; private set; }

        /// <summary>
        /// Gets a protocol method to be used in the request.
        /// If the property returns null or blank string the client will try to deduce the method from the query type using the next rule:
        /// if there is RequestSerializationAttribute defined either on the TQuery class or one of its properties or on the HttpClient's subclass then the method will be POST
        /// otherwise it'll use GET. The default value is null.
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
        /// Gets the service configuration. If it's not defined then the default values will be used.
        /// </summary>
        protected HttpClientEndpointElement Configuration { get; private set; }

        /// <summary>
        /// Retry strategy for calling the remote endpoint.
        /// </summary>
        protected Func<Func<TResult>, TResult> RetryStrategy { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HttpClient.
        /// </summary>
        /// <param name="serviceUrl">The URL of the service.</param>
        /// <param name="configurationName">If the configuration name is not null it'll be used to get the endpoint configuration from the config file.</param>
        /// <param name="retryStrategy">If the parameter null then the default retry strategy will be used.</param>
        public HttpClient(Uri serviceUrl = null, string configurationName = null, Func<Func<TResult>, TResult> retryStrategy = null)
        {
            BaseUrl = serviceUrl;
            RetryStrategy = retryStrategy ?? GetDefaultRetryStrategy();
            AutoSetAcceptEncoding = true;

            ReadConfiguration(configurationName);

            if(BaseUrl == null)
                throw new ArgumentException(Resources.Text.service_url_is_not_defined, "serviceUrl");
        }

        /// <summary>
        /// Executes a http request. By default all properties with public getters are serialized into the http query part.
        /// The query class may define some properties to be serialized into the http query part and to serialize some property
        /// into the request body.
        /// The input data serialization behaviour can be altered by:
        ///   * Using QueryIgnoreAttribute (on class or property level),
        ///   * Using one of the RequestSerializationAttribute derived classes (on the class or property level) 
        ///   * Implementing ICustomQueryMapper interface for custom mapping to query string
        ///   * Overriding TryMakeQueryString and/or TryWriteRequestData
        /// The output data deserialization behaviour can be altered by:
        ///   * Using one of the ResponseDeserializationAttribute derived classes (on the class level)
        ///   * Adding or replacing existing deserialization providers in the ResponseParser static class
        ///   * Overriding ParseResponse
        /// The remote call is wrapped into the retry strategy.
        /// </summary>
        /// <param name="query">Input parameters.</param>
        /// <returns>Output data.</returns>
        public TResult Execute(TQuery query)
        {
            try
            {
                return RetryStrategy(() => ExecutePipeline(query));
            }
            catch (HttpClientException)
            {
                throw;
            }
            catch(Exception e)
            {
                throw new HttpClientException(string.Format(Resources.Text.failed_execute_request, BaseUrl, GetType().FullName), e);
            }
        }

        /// <summary>
        /// Executes the actual request execution pipeline.
        ///     1. Builds full URL using the query class by calling UrlBuilder.CreateUrl(BaseUrl, query)
        ///     2. Creates web request (if headers, client certificates, and a proxy are defined in the configuration they will be added to the request)
        ///     3. Writes to the request's body if there is something to write
        ///     4. Gets response from the remote server and parses it
        /// </summary>
        protected virtual TResult ExecutePipeline(TQuery query)
        {
            var url = BuildUrl(query);

            var request = CreateRequest(url);

            TryWriteRequestData(query, request);

            return ParseResponse(query, request);
        }

        /// <summary>
        /// Builds a full URL from the BaseUrl and the query object. The method return string instead of Uri for precise control
        /// which may be required in case of URL signing like for some Google services.
        /// </summary>
        /// <returns></returns>
        protected virtual string BuildUrl(TQuery query)
        {
            return UrlBuilder.CreateUrl(BaseUrl, query).ToString();
        }

        /// <summary>
        /// Creates the request. If headers, client certificates, and a proxy are defined in the configuration they will be added to the request
        /// </summary>
        protected virtual WebRequest CreateRequest(string url)
        {
            var request = WebRequest.Create(url);

            request.Timeout = RequestTimeout;
            request.Method = GetRequestMethod();

            if (AutoSetAcceptEncoding && (!typeof(TResult).IsAssignableFrom(typeof(Image))))
                ContentDecoderFactory.AddAcceptEncodings(request);

            Configuration.AddHeaders(request);

            Configuration.AddClientCertificates(request as HttpWebRequest);

            Configuration.SetWebProxy(request);

            return request;
        }

        /// <summary>
        /// Gets the request method (GET,PUT, etc.). If RequestMethod is null or blank then tries to figure out what method to use
        /// by checking the query type.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Tries to write data to the request's body by examining the query type.
        /// </summary>
        protected virtual void TryWriteRequestData(TQuery query, WebRequest request)
        {
            var serializer = RequestSerializationFactory.GetSerializer(this, query);
            if(serializer != null)
                serializer.Serialize(query, request);
        }

        /// <summary>
        /// Gets the response and parses it. 
        /// </summary>
        protected virtual TResult ParseResponse(TQuery query, WebRequest request)
        {
            return (TResult)ResponseParser.Parse(request, typeof(TResult));
        }

        /// <summary>
        /// Simple retry strategy to call the remote server, if the call fails it will be retried the defines number of times after each time increasing the timeout by stepbackIncrease.
        /// </summary>
        protected TResult DefaultRetryStrategy(Func<TResult> action, int retries, int initialTimeout, int stepbackIncrease)
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
                    if (retries <= 0)
                        throw;

                    OnLogRetry(++attempt, e);

                    --retries;

                    Thread.Sleep(timeout);

                    timeout += stepbackIncrease;
                }
            }
        }
    
        /// <summary>
        /// Is called each time before retry. The default implementation uses Debug.Write.
        /// </summary>
        protected virtual void OnLogRetry(int attempt, Exception e)
        {
            if(e == null)
                Debug.Write(string.Format(Resources.Text.will_try_again, attempt));
            else
                Debug.WriteLine("{0} : {1}", string.Format(Resources.Text.will_try_again, attempt), e);
        }

        /// <summary>
        /// Gets's the configured default strategy. It has 3 retries with initial timeout of 1 sec and step back of 1 sec.
        /// So the timeouts will be: 1 sec after the initial call, 2 sec after the first retry, 3 sec after the second retry.
        /// </summary>
        protected Func<Func<TResult>, TResult> GetDefaultRetryStrategy()
        {
            return action => DefaultRetryStrategy(action, 3, 1000, 1000);
        }
       
        void ReadConfiguration(string configurationName)
        {
            Configuration = GetConfigurationElement(configurationName);

            if (Configuration.BaseUrl != null)
                BaseUrl = Configuration.BaseUrl;

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
