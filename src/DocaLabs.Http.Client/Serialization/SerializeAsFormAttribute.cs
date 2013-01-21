using System.Net;
using System.Text;
using DocaLabs.Http.Client.Mapping;

namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Serializes a given object into the web request as Url encoded form (the content type is: application/x-www-form-urlencoded).
    /// The class uses QueryMapper.ToQueryString for serialization, the string is encoded as UTF8.
    /// </summary>
    public class SerializeAsFormAttribute : RequestSerializationAttribute
    {
        /// <summary>
        /// Serializes a given object into the web request as Url encoded form (the content type is: application/x-www-form-urlencoded).
        /// </summary>
        /// <param name="obj">Object to be serialized.</param>
        /// <param name="request">Web request where to serialize to.</param>
        public override void Serialize(object obj, WebRequest request)
        {
            var data = Encoding.UTF8.GetBytes(QueryMapper.ToQueryString(obj));

            request.ContentType = "application/x-www-form-urlencoded";

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
        }
    }
}
