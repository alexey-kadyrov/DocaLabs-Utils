using System;
using System.Xml;
using System.Xml.Serialization;

namespace DocaLabs.Http.Client.Deserialization
{
    /// <summary>
    /// Deserializes the response stream content using XML format.
    /// </summary>
    public class XmlResponseDeserializer : IResponseDeserializationProvider
    {
        /// <summary>
        /// Deserializes the response stream content using JSON format.
        /// The method is using XmlSerializer with default settings except the DTD processing is set to ignore.
        /// </summary>
        public TResult Deserialize<TResult>(HttpResponse response)
        {
            // stream is disposed by the reader
            using (var reader = XmlReader.Create(response.GetDataStream(), new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
            {
                return (TResult)new XmlSerializer(typeof(TResult)).Deserialize(reader);
            }
        }

        /// <summary>
        /// Returns true if the content type is 'text/xml' and the TResult is not "simple type", like int, string, Guid, double, etc.
        /// </summary>
        public bool CheckIfSupports<TResult>(HttpResponse response)
        {
            return 
                (!string.IsNullOrWhiteSpace(response.ContentType)) &&
                string.Compare(response.ContentType, "text/xml", StringComparison.OrdinalIgnoreCase) == 0 &&
                (!typeof(TResult).IsSimpleType());
        }
    }
}
