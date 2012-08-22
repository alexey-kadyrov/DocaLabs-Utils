using System;
using System.Net;
using DocaLabs.Http.Client.Configuration;
using NLog;

namespace DocaLabs.Http.Client
{
    /// <summary>
    /// Base class for RESTFull service clients.
    /// </summary>
    public abstract class HttpClientBase
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets a service Url, must be overridden in derived classes.
        /// </summary>
        public Uri ServiceUrl { get; private set; }

        /// <summary>
        /// Gets or sets the request timeout, the default value is -1.
        /// </summary>
        public int RequestTimeout { get; set; }

        /// <summary>
        /// Gets a protocol method used in the request. The default value is GET.
        /// </summary>
        public virtual string RequestMethod { get { return null; } }

        /// <summary>
        /// Gets the service configuration if it's defined in the config file, otherwise null.
        /// </summary>
        protected HttpClientEndpointElement Configuration { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HttpClientBase class.
        /// </summary>
        protected HttpClientBase()
        {
            ReadConfiguration(GetDefaultEndPointName());
        }

        /// <summary>
        /// Initializes a new instance of the HttpClientBase class using the specified service Url.
        /// </summary>
        protected HttpClientBase(Uri serviceUrl)
        {
            ServiceUrl = serviceUrl;
            ReadConfiguration(GetDefaultEndPointName());
        }

        /// <summary>
        /// Initializes a new instance of the HttpClientBase class using the specified service Url and the endpoint name.
        /// The endpoint name is used to get the endpoint configuration from the config file.
        /// </summary>
        protected HttpClientBase(Uri serviceUrl, string endpointName)
        {
            ServiceUrl = serviceUrl;
            ReadConfiguration(endpointName);
        }

        void ReadConfiguration(string endpointName)
        {
            Configuration = GetConfigurationElement(endpointName);

            if (Configuration == null)
            {
                RequestTimeout = -1;
                return;
            }

            if (ServiceUrl == null)
                ServiceUrl = Configuration.ServiceUrl;

            RequestTimeout = Configuration.Timeout;
        }

        static HttpClientEndpointElement GetConfigurationElement(string endpointName)
        {
            if (string.IsNullOrWhiteSpace(endpointName))
                return null;

            var section = HttpClientEndpointSection.GetDefaultSection();
            return section != null 
                ? section.Endpoints[endpointName] 
                : null;
        }

        string GetDefaultEndPointName()
        {
            return GetType().FullName;
        }
    }
}