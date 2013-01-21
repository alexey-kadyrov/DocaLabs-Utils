using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Serializes a given object into the web request in json format, the string is encoded as UTF8.
    /// </summary>
    public class SerializeAsJsonAttribute : RequestSerializationAttribute
    {
        /// <summary>
        /// Serializes a given object into the web request in json format
        /// </summary>
        /// <param name="obj">Object to be serialized.</param>
        /// <param name="request">Web request where to serialize to.</param>
        public override void Serialize(object obj, WebRequest request)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));

            request.ContentType = "application/json";

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
        }
    }
}
