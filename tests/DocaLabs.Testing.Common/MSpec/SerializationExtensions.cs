using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DocaLabs.Testing.Common.MSpec
{
    public static class SerializationExtensions
    {
        public static T ParseXml<T>(this Stream stream)
        {
            using (var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
            }
        }

        public static T ParseXml<T>(this byte[] xml)
        {
            using (var stream = new MemoryStream(xml))
            using (var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
            }
        }

        public static T ParseJson<T>(this byte[] data)
        {
            return Encoding.UTF8.GetString(data).ParseJson<T>();
        }

        public static T ParseJson<T>(this string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
