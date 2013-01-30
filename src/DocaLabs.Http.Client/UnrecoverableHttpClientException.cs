using System;
using System.Runtime.Serialization;

namespace DocaLabs.Http.Client
{
    [Serializable]
    public class UnrecoverableHttpClientException : HttpClientException
    {
        public UnrecoverableHttpClientException()
            : base("UnrecoverableHttpClientException was thrown.")
        {
        }

        public UnrecoverableHttpClientException(string message)
            : base(message)
        {
        }

        public UnrecoverableHttpClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UnrecoverableHttpClientException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
