using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Net;

namespace DocaLabs.Http.Client.Configuration
{
    /// <summary>
    /// Represents a configuration element that defines the http client endpoint. 
    /// </summary>
    public class HttpClientEndpointElement : ConfigurationElement
    {
        const string NameProperty = "name";
        const string ServiceUrlProperty = "serviceUrl";
        const string TimeoutProperty = "timeout";
        const string HeadersProperty = "headers";
        const string ClientCertificatesProperty = "clientCertificates";
        const string ProxyProperty = "proxy";

        /// <summary>
        /// Always returns false letting the element to be modified at runtime.
        /// </summary>
        /// <returns></returns>
        public override bool IsReadOnly()
        {
            return false;
        }

        /// <summary>
        /// Gets or sets the endpoint name.
        /// </summary>
        [ConfigurationProperty(NameProperty, IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return ((string)base[NameProperty]); }
            set { base[NameProperty] = value; }
        }

        /// <summary>
        /// Gets or sets the ServiceUrl.
        /// </summary>
        [ConfigurationProperty(ServiceUrlProperty, IsRequired = false), TypeConverter(typeof(UriTypeConverter))]
        public Uri ServiceUrl
        {
            get { return ((Uri)base[ServiceUrlProperty]); }
            set { base[ServiceUrlProperty] = value; }
        }

        /// <summary>
        /// Gets or sets the request timeout. Default value is 90 seconds.
        /// </summary>
        [ConfigurationProperty(TimeoutProperty, IsRequired = false, DefaultValue = 90000)]
        public int Timeout
        {
            get { return ((int)base[TimeoutProperty]); }
            set { base[TimeoutProperty] = value; }
        }

        /// <summary>
        /// Gets the headers collection.
        /// </summary>
        [ConfigurationProperty(HeadersProperty, IsRequired = false)]
        public NameValueConfigurationCollection Headers
        {
            get { return ((NameValueConfigurationCollection)base[HeadersProperty]); }
        }

        /// <summary>
        /// Gets the client certificate collection.
        /// </summary>
        [ConfigurationProperty(ClientCertificatesProperty, IsRequired = false)]
        public HttpClientCertificateCollection ClientCertificates
        {
            get { return ((HttpClientCertificateCollection)base[ClientCertificatesProperty]); }
        }

        /// <summary>
        /// Gets the proxy.
        /// </summary>
        [ConfigurationProperty(ProxyProperty, IsRequired = false)]
        public HttpClientProxyElement Proxy
        {
            get { return ((HttpClientProxyElement)base[ProxyProperty]); }
        }

        public void AddHeaders(WebRequest request)
        {
            if (request == null)
                return;

            foreach (var name in Headers.AllKeys)
                request.Headers.Add(name, Headers[name].Value);
        }

        public void AddClientCertificates(HttpWebRequest request)
        {
            if (request == null)
                return;

            foreach (HttpClientCertificateReferenceElement certRef in ClientCertificates)
                request.ClientCertificates.AddRange(certRef.Find());
        }

        public void SetWebProxy(WebRequest request)
        {
            if (request != null && Proxy.Address != null)
                request.Proxy = new WebProxy(Proxy.Address);
        }
    }
}
