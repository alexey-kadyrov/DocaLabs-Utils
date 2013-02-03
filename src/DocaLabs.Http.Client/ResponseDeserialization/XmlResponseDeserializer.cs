using System;
using System.Xml;
using System.Xml.Serialization;

namespace DocaLabs.Http.Client.ResponseDeserialization
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
        public object Deserialize(HttpResponse response, Type resultType)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            if (resultType == null)
                throw new ArgumentNullException("resultType");

            // stream is disposed by the reader
            using (var reader = XmlReader.Create(response.GetDataStream(), new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
            {
                return new XmlSerializer(resultType).Deserialize(reader);
            }
        }

        /// <summary>
        /// Returns true if the content type is 'text/xml' and the TResult is not "simple type", like int, string, Guid, double, etc.
        /// </summary>
        public bool CanDeserialize(HttpResponse response, Type resultType)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            if (resultType == null)
                throw new ArgumentNullException("resultType");

            return 
                (!string.IsNullOrWhiteSpace(response.ContentType)) &&
                string.Compare(response.ContentType, "text/xml", StringComparison.OrdinalIgnoreCase) == 0 &&
                (!resultType.IsSimpleType());
        }
    }
}
