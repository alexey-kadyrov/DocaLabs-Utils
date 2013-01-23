using System.IO;
using System.Net;
using System.Text;
using DocaLabs.Http.Client.Mapping;
using DocaLabs.Http.Client.Serialization.ContentEncoding;

namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Serializes a given object into the web request as Url encoded form (the content type is: application/x-www-form-urlencoded).
    /// The class uses QueryMapper.ToQueryString for serialization, the string is encoded as UTF8.
    /// </summary>
    public class SerializeAsFormAttribute : RequestSerializationAttribute
    {
        /// <summary>
        /// Gets or sets the content encoding, if ContentEncoding blank or null no encoding is done.
        /// The encoder is supplied by ContentEncoderFactory.
        /// </summary>
        public string RequestContentEncoding { get; set; }

        /// <summary>
        /// Serializes a given object into the web request as Url encoded form (the content type is: application/x-www-form-urlencoded).
        /// </summary>
        /// <param name="obj">Object to be serialized.</param>
        /// <param name="request">Web request where to serialize to.</param>
        public override void Serialize(object obj, WebRequest request)
        {
            var data = Encoding.UTF8.GetBytes(QueryMapper.ToQueryString(obj));

            request.ContentType = "application/x-www-form-urlencoded";

            if (string.IsNullOrWhiteSpace(RequestContentEncoding))
                Write(data, request);
            else
                EncodeAndWrite(data, request);
        }

        static void Write(byte[] data, WebRequest request)
        {
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
        }

        void EncodeAndWrite(byte[] data, WebRequest request)
        {
            request.Headers.Add(string.Format("content-encoding: {0}", RequestContentEncoding));

            using (var requestStream = request.GetRequestStream())
            using (var compressionStream = ContentEncoderFactory.Get(RequestContentEncoding).GetCompressionStream(requestStream))
            using (var dataStream = new MemoryStream(data))
            {
                dataStream.CopyTo(compressionStream);
            }
        }
    }
}
