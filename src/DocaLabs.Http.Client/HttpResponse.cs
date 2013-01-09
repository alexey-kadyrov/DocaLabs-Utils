using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DocaLabs.Http.Client.ContentEncoding;
using Newtonsoft.Json;

namespace DocaLabs.Http.Client
{
    /// <summary>
    /// A wrapper around WebResponse instance.
    /// </summary>
    public class HttpResponse : IDisposable
    {
        /// <summary>
        /// Gets the underlying response object.
        /// </summary>
        public WebResponse Response { get; private set; }

        /// <summary>
        /// Gets a response stream.
        /// </summary>
        public Stream RawResponseStream { get; private set; }

        /// <summary>
        /// Initializes an instance of the HttpResponse class with provided WebRequest instance.
        /// </summary>
        public HttpResponse(WebResponse response)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            Response = response;

            RawResponseStream = Response.GetResponseStream();
            if (RawResponseStream == null)
                throw new HttpClientException(Resources.Text.null_response_stream);
        }

        /// <summary>
        /// Returns true if the content type is 'application/json'.
        /// </summary>
        public bool IsJson()
        {
            return (!string.IsNullOrWhiteSpace(Response.ContentType)) &&
                string.Compare(Response.ContentType, "application/json", StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns true if the content type is 'text/xml'.
        /// </summary>
        public bool IsXml()
        {
            return (!string.IsNullOrWhiteSpace(Response.ContentType)) &&
                string.Compare(Response.ContentType, "text/xml", StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns the content of the response stream as a byte array.
        /// </summary>
        public byte[] AsByteArray()
        {
            var memoryStream = new MemoryStream();

            using (var stream = GetDataStream())
            {
                stream.CopyTo(memoryStream);
            }

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Returns the content of the response stream as a string.
        /// If the encoding cannot be inferred from the response then UTF-8 is assumed.
        /// </summary>
        public string AsString()
        {
            // stream is disposed by the reader
            using (var reader = new StreamReader(GetDataStream(), TryGetEncoding()))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Deserializes the response stream content using JSON format.
        /// The method is using Newtonsoft deserializer with default settings.
        /// If the response stream content is empty then the default(T) is returned.
        /// </summary>
        public T AsJsonObject<T>()
        {
            var s = AsString();

            return string.IsNullOrWhiteSpace(s) 
                ? default(T) 
                : JsonConvert.DeserializeObject<T>(s);
        }

        /// <summary>
        /// Deserializes the response stream content using JSON format.
        /// The method is using XmlSerializer with default settings except the DTD processing is set to ignore.
        /// </summary>
        public T AsXmlObject<T>()
        {
            // stream is disposed by the reader
            using (var reader = XmlReader.Create(GetDataStream(), new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
            }
        }

        /// <summary>
        /// Tries to infer the response encoding for HttpWebResponse or returns UTF-8 otherwise.
        /// </summary>
        /// <returns></returns>
        public Encoding TryGetEncoding()
        {
            try
            {
                var httpResponse = Response as HttpWebResponse;

                return httpResponse == null || string.IsNullOrWhiteSpace(httpResponse.CharacterSet) 
                    ? Encoding.UTF8
                    : Encoding.GetEncoding(httpResponse.CharacterSet);
            }
            catch
            {
                return Encoding.UTF8;
            }
        }

        /// <summary>
        /// Returns the response stream, if the content is encoded (compressed) then it will be decoded using decoder provided by CurrentEncodingProvider.Current.
        /// </summary>
        /// <returns>RawResponseStream or a stream containing decoded content.</returns>
        public Stream GetDataStream()
        {
            var httpResponse = Response as HttpWebResponse;
            if (httpResponse == null || string.IsNullOrWhiteSpace(httpResponse.ContentEncoding))
                return RawResponseStream;

            return CurrentEncodingProvider.Current.GetDecoder(httpResponse.ContentEncoding).Decode(RawResponseStream);
        }

        /// <summary>
        /// Releases the response and the stream.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases the response and the stream.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing == false)
                return;

            if(RawResponseStream != null)
                RawResponseStream.Dispose();

            if(Response != null)
                Response.Close();
        }
    }
}
