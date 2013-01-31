using Newtonsoft.Json;

namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Deserializes json serialized object from the web response.
    /// </summary>
    public class DeserializeFromJsonAttribute : ResponseDeserializationAttribute
    {
        /// <summary>
        /// Deserializes json serialized object from the web response.
        /// </summary>
        public override T Deserialize<T>(HttpResponse response)
        {
            var s = response.AsString();

            return string.IsNullOrWhiteSpace(s)
                ? default(T)
                : JsonConvert.DeserializeObject<T>(s);
        }
    }
}
