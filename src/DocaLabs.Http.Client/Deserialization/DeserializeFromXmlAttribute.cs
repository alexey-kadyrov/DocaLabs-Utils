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
        public override T Deserialize<T>(HttpResponse response)
        {
            // stream is disposed by the reader
            using (var reader = XmlReader.Create(response.GetDataStream(), new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
            }
        }
    }
}
