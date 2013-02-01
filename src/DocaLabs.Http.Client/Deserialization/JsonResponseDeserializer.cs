using System;
using Newtonsoft.Json;

namespace DocaLabs.Http.Client.Deserialization
{
    /// <summary>
    /// Deserializes the response stream content using JSON format.
    /// </summary>
    public class JsonResponseDeserializer : IResponseDeserializationProvider
    {
        /// <summary>
        /// Deserializes the response stream content using JSON format.
        /// The method is using Newtonsoft deserializer with default settings.
        /// If the response stream content is empty then the default(TResult) is returned.
        /// </summary>
        public object Deserialize(HttpResponse response, Type resultType)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            if (resultType == null)
                throw new ArgumentNullException("resultType");

            var s = response.AsString();

            return string.IsNullOrWhiteSpace(s)
                ? null
                : JsonConvert.DeserializeObject(s, resultType);
        }

        /// <summary>
        /// Returns true if the content type is 'application/json' and the TResult is not "simple type", like int, string, Guid, double, etc.
        /// </summary>
        public bool CheckIfSupports(HttpResponse response, Type resultType)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            if (resultType == null)
                throw new ArgumentNullException("resultType");

            return 
                (!string.IsNullOrWhiteSpace(response.ContentType)) &&
                string.Compare(response.ContentType, "application/json", StringComparison.OrdinalIgnoreCase) == 0 && 
                (!resultType.IsSimpleType());
        }
    }
}
