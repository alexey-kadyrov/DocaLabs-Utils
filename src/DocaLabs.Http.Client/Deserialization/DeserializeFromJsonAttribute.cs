using System;
using Newtonsoft.Json;

namespace DocaLabs.Http.Client.Deserialization
{
    /// <summary>
    /// Deserializes json serialized object from the web response.
    /// </summary>
    public class DeserializeFromJsonAttribute : ResponseDeserializationAttribute
    {
        /// <summary>
        /// Deserializes json serialized object from the web response.
        /// </summary>
        public override object Deserialize(HttpResponse response, Type resultType)
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
    }
}
