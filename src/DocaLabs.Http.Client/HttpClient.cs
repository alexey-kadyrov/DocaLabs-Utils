using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using DocaLabs.Http.Client.Configuration;
using DocaLabs.Http.Client.Mapping;
using DocaLabs.Http.Client.Serialization;
using DocaLabs.Http.Client.Serialization.ContentEncoding;

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
    public abstract class HttpClient<TQuery, TResult> : HttpClientBase
    {
        protected Func<Func<TResult>, TResult> RetryStrategy { get; private set; }

        protected HttpClient(Uri serviceUrl = null, Func<Func<TResult>, TResult> retryStrategy = null, string configurationName = null)
            : base(serviceUrl, configurationName)
        {
            RetryStrategy = retryStrategy ?? GetDefaultRetryStrategy();
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
        ///   * Overriding OnResultTransforming
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
                var message = string.Format(Resources.Text.failed_execute_request, ServiceUrl, GetType().FullName);

                LogException(message, e);

                if (e is HttpClientException)
                    throw;

                throw new HttpClientException(message, e);
            }
        }

        protected virtual TResult DoExecute(TQuery query)
        {
            var url = CreateUrl(query);

            var request = CreateRequest(query, url);

            TryWriteRequestData(query, request);

            return ParseResponse(query, request);
        }

        protected virtual Uri CreateUrl(TQuery query)
        {
            try
            {
                return new UriBuilder(ServiceUrl)
                {
                    Query = TryMakeQueryString(query)
                }.Uri;
            }
            catch (Exception e)
            {
                throw new HttpClientException(string.Format(Resources.Text.failed_create_url, ServiceUrl), e)
                {
                    DoNotRetry = true
                };
            }
        }

        protected virtual WebRequest CreateRequest(TQuery query, Uri url)
        {
            var request = WebRequest.Create(url);

            request.Timeout = RequestTimeout;
            request.Method = GetRequestMethod();

            var headers = GetHeaders(query, url);
            if(headers != null && headers.Count > 0)
                request.Headers.Add(headers);

            var httpRequest = request as HttpWebRequest;
            if (httpRequest != null)
            {
                var certificates = GetClientCertificates(query, url);
                if(certificates != null)
                    httpRequest.ClientCertificates.AddRange(certificates);
            }

            var proxy = GetWebProxy(query, url);
            if (proxy != null)
                request.Proxy = proxy;

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

        protected virtual NameValueCollection GetHeaders(TQuery query, Uri url)
        {
            var collection = new NameValueCollection();

            var supportedDecoders = ContentDecoderFactory.GetSupportedEncodings();
            if (supportedDecoders.Count > 0)
                collection.Add("Accept-Encoding", string.Join(", ", supportedDecoders));    

            if (Configuration != null)
            {
                foreach (var name in Configuration.Headers.AllKeys)
                    collection.Add(name, Configuration.Headers[name].Value);
            }

            return collection;
        }

        protected virtual X509CertificateCollection GetClientCertificates(TQuery query, Uri url)
        {
            if (Configuration == null)
                return null;

            var collection = new X509CertificateCollection();

            foreach (HttpClientCertificateReferenceElement certRef in Configuration.ClientCertificates)
                collection.AddRange(certRef.Find());

            return collection;
        }

        protected virtual IWebProxy GetWebProxy(TQuery query, Uri url)
        {
            if (Configuration == null || Configuration.Proxy.Address == null)
                return null;

            return new WebProxy(Configuration.Proxy.Address);
        }

        protected virtual string TryMakeQueryString(TQuery query)
        {
            return typeof(TQuery).GetCustomAttributes(typeof(QueryIgnoreAttribute), true).Length == 0
                ? QueryMapper.ToQueryString(query)
                : "";
        }

        protected virtual void TryWriteRequestData(TQuery query, WebRequest request)
        {
            if (TryQueryClassLevel(query, request)) 
                return;

            if (TryQueryPropertyLevel(query, request))
                return;

            TryHttpClientClassLevel(query, request);
        }

        protected virtual bool TryQueryClassLevel(TQuery query, WebRequest request)
        {
            var attrs = typeof(TQuery).GetCustomAttributes(typeof(RequestSerializationAttribute), true);
            if (attrs.Length == 0)
                return false;

            ((RequestSerializationAttribute) attrs[0]).Serialize(query, request);

            return true;
        }

        protected virtual bool TryQueryPropertyLevel(TQuery query, WebRequest request)
        {
            foreach (var property in typeof(TQuery).GetProperties())
            {
                var attrs = property.GetCustomAttributes(typeof(RequestSerializationAttribute), true);
                if (attrs.Length > 0)
                {
                    ((RequestSerializationAttribute)attrs[0]).Serialize(property.GetValue(query, null), request);
                    return true;
                }
            }

            return false;
        }

        protected virtual bool TryHttpClientClassLevel(TQuery query, WebRequest request)
        {
            var attrs = GetType().GetCustomAttributes(typeof(RequestSerializationAttribute), true);
            if (attrs.Length == 0)
                return false;

            ((RequestSerializationAttribute)attrs[0]).Serialize(query, request);

            return true;
        }

        protected virtual TResult ParseResponse(TQuery query, WebRequest request)
        {
            using (var response = GetResponse(request))
            {
                return OnResultTransforming(query, response);
            }
        }

        protected virtual HttpResponse GetResponse(WebRequest request)
        {
            return new HttpResponse(request.GetResponse());
        }

        protected virtual TResult OnResultTransforming(TQuery query, HttpResponse response)
        {
            var attrs = typeof(TResult).GetCustomAttributes(typeof(ResponseDeserializationAttribute), true);
            if (attrs.Length > 0)
                return ((ResponseDeserializationAttribute)attrs[0]).Deserialize<TResult>(response);

            if (response.IsJson())
                return response.AsJsonObject<TResult>();

            if (response.IsXml())
                return response.AsXmlObject<TResult>();

            throw new HttpClientException(string.Format(Resources.Text.method_must_be_overidden, GetType().FullName))
            {
                DoNotRetry = true
            };
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
                catch (Exception e)
                {
                    attempt++;

                    if (attempt >= times)
                        throw;

                    if (e is HttpClientException && ((HttpClientException)e).DoNotRetry)
                        throw;

                    LogWarning(string.Format(Resources.Text.will_try_again, attempt, times), e);

                    Thread.Sleep(timeout);

                    timeout += stepbackIncrease;
                }
            }
        }

        protected Func<Func<TResult>, TResult> GetDefaultRetryStrategy()
        {
            return action => DefaultRetryStrategy(action, 4, 1000, 1000);
        }
    
        protected virtual void LogWarning(string message, Exception e)
        {
            if(e == null)
                Debug.Write(message);
            else
                Debug.WriteLine("{0} : {1}", message, e);
        }

        protected virtual void LogException(string message, Exception e)
        {
            Debug.WriteLine("{0} : {1}", message, e);
        }
    }
}
