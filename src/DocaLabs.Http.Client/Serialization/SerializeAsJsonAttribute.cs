using System.IO;
using System.Net;
using System.Text;
using DocaLabs.Http.Client.Serialization.ContentEncoding;
using Newtonsoft.Json;

namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Serializes a given object into the web request in json format, the string is encoded as UTF8.
    /// </summary>
    public class SerializeAsJsonAttribute : RequestSerializationAttribute
    {
        /// <summary>
        /// Gets or sets the content encoding, if ContentEncoding blank or null no encoding is done.
        /// The encoder is supplied by ContentEncoderFactory.
        /// </summary>
        public string RequestContentEncoding { get; set; }

        /// <summary>
        /// Serializes a given object into the web request in json format
        /// </summary>
        /// <param name="obj">Object to be serialized.</param>
        /// <param name="request">Web request where to serialize to.</param>
        public override void Serialize(object obj, WebRequest request)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));

            request.ContentType = "application/json";
            
            if(string.IsNullOrWhiteSpace(RequestContentEncoding))
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
            using (var dataStream = new MemoryStream(data))
            using (var compressionStream = ContentEncoderFactory.Get(RequestContentEncoding).GetCompressionStream(dataStream))
            {
                compressionStream.CopyTo(requestStream);
            }
        }
    }
}
