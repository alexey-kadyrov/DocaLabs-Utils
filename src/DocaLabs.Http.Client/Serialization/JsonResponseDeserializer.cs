using System;
using Newtonsoft.Json;

namespace DocaLabs.Http.Client.Serialization
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
        public TResult Deserialize<TResult>(HttpResponse response)
        {
            var s = response.AsString();

            return string.IsNullOrWhiteSpace(s)
                ? default(TResult)
                : JsonConvert.DeserializeObject<TResult>(s);
        }

        /// <summary>
        /// Returns true if the content type is 'application/json' and the TResult is not "simple type", like int, string, Guid, double, etc.
        /// </summary>
        public bool CheckIfSupports<TResult>(HttpResponse response)
        {
            return 
                (!string.IsNullOrWhiteSpace(response.ContentType)) &&
                string.Compare(response.ContentType, "application/json", StringComparison.OrdinalIgnoreCase) == 0 && 
                (!typeof(TResult).IsSimpleType());
        }
    }
}
