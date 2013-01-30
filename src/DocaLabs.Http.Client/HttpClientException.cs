using System;
using System.Runtime.Serialization;

namespace DocaLabs.Http.Client
{
    [Serializable]
    public class HttpClientException : Exception
    {
        public HttpClientException()
            : base("HttpClientException was thrown.")
        {
        }

        public HttpClientException(string message)
            : base(message)
        {
        }

        public HttpClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected HttpClientException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
