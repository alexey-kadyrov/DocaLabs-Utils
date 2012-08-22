using System;
using System.Collections.Concurrent;

namespace DocaLabs.Http.Client.Factory
{
    /// <summary>
    /// Defines the base class that is used by factory to generate http clients from interface definitions.
    /// </summary>
    public abstract class HttpClientAccessBase
    {
        ConcurrentDictionary<Type, object> Instances { get; set; }

        /// <summary>
        /// Initializes a new instance of the HttpClientAccessBase class.
        /// </summary>
        protected HttpClientAccessBase()
        {
            Instances = new ConcurrentDictionary<Type, object>();
        }

        /// <summary>
        /// Gets or creates a new instance of the HttpClient with specified serviceUrl, method and the endpoint name in the config file.
        /// </summary>
        /// <typeparam name="TQuery">Type which will be used as input parameters that can be serialized into query string or the request stream.</typeparam>
        /// <typeparam name="TResult">Type which will be used as output data that will be deserialized from the response stream.</typeparam>
        /// <param name="serviceUrl">Service base Url, if the argument is null than it must be defined in the configuration referenced by the endpoint name.</param>
        /// <param name="method">The request method, like GET,POST,PUT,DELETE. If it's null or empty than the HttpClient will try to infer it from the query.</param>
        /// <param name="enpointName">The endpoint name in the config file. The argument is optional.</param>
        /// <returns></returns>
        protected HttpClient<TQuery, TResult> GetInstance<TQuery, TResult>(Uri serviceUrl, string method, string enpointName)
        {
            return (HttpClient<TQuery, TResult>)Instances.GetOrAdd(
                typeof(HttpClient<TQuery, TResult>), x => CreateInstance<TQuery, TResult>(serviceUrl, method, enpointName));
        }

        static HttpClient<TQuery, TResult> CreateInstance<TQuery, TResult>(Uri serviceUrl, string method, string enpointName)
        {
            return new HttpClientInstance<TQuery, TResult>(serviceUrl, method, enpointName);
        }

        class HttpClientInstance<TQuery, TResult> : HttpClient<TQuery, TResult>
        {
            readonly string _method;
            public override string RequestMethod { get { return _method; } }

            public HttpClientInstance(Uri serviseUrl, string method, string endpointName)
                : base(serviseUrl, GetDefaultRetryStrategy(), endpointName)
            {
                _method = method;
            }
        }
    }
}