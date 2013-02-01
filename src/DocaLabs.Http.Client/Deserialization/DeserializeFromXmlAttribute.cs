using System;
using System.Xml;
using System.Xml.Serialization;

namespace DocaLabs.Http.Client.Deserialization
{
    /// <summary>
    /// Deserializes xml serialized object from the web response.
    /// </summary>
    public class DeserializeFromXmlAttribute : ResponseDeserializationAttribute
    {
        /// <summary>
        /// Deserializes xml serialized object from the web response.
        /// </summary>
        public override object Deserialize(HttpResponse response, Type resultType)
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
    }
}
