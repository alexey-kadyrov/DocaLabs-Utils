using System;

namespace DocaLabs.Http.Client.Factory
{
    /// <summary>
    /// Used by the HttpClientFactory to override default behaviour and get additional information about service that is linked to a method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HttpServiceAttribute : Attribute
    {
        /// <summary>
        /// The protocol method, if the property is null or empty then the client will try to infer the method from the query class.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Service Url, if the property is null or empty than it's expected that the Url will be defined in the config file.
        /// </summary>
        public string ServiceUrl { get; set; }
    }
}
