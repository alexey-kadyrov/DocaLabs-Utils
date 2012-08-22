using System;
using System.Runtime.Serialization;

namespace DocaLabs.Http.Client.Factory
{
    /// <summary>
    /// Represents an exception that is thrown by the HttpClientFactory.
    /// </summary>
    [Serializable]
    public class HttpClientFactoryException : HttpClientException
    {
        /// <summary>
        /// Initializes a new instance of the HttpClientFactoryException class.
        /// </summary>
        public HttpClientFactoryException()
        {
            DoNotRetry = true;
        }

        /// <summary>
        /// Initializes a new instance of the HttpClientFactoryException class with a specified error message.
        /// </summary>
        public HttpClientFactoryException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpClientFactoryException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public HttpClientFactoryException(string message, Exception inner) 
            : base(message, inner)
        {
            DoNotRetry = true;
        }

        /// <summary>
        /// Initializes a new instance of the HttpClientFactoryException class with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        protected HttpClientFactoryException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
            DoNotRetry = true;
        }
    }
}
