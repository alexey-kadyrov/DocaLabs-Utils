using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DocaLabs.Http.Client
{
    [Serializable]
    public class HttpClientException : Exception
    {
        const string DoNotRetryValueName = "DoNotRetry";

        public bool DoNotRetry { get; set; }

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
            DoNotRetry = info.GetBoolean(DoNotRetryValueName);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(DoNotRetryValueName, DoNotRetry);

            base.GetObjectData(info, context);
        }
    }
}
