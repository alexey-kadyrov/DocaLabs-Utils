using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Serializes a given object into the web request in xml format.
    /// The class uses XmlSerializer for serialization.
    /// </summary>
    public class SerializeAsXmlAttribute : RequestSerializationAttribute
    {
        /// <summary>
        /// Gets or sets the type of text encoding to be used for Xml serialization.
        /// The default value is UTF-8.
        /// </summary>
        public CharEncoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to indent elements.
        /// The default values is true.
        /// </summary>
        public bool Indent { get; set; }

        /// <summary>
        /// Gets or sets the character string to use when indenting. This setting is used when the Indent property is set to true.
        /// The default value is the tab symbol.
        /// </summary>
        public string IndentChars { get; set; }

        /// <summary>
        /// Gets or sets the name of the DOCTYPE if one to be used for serialized Xml.
        /// </summary>
        public string DocTypeName { get; set; }

        /// <summary>
        /// If non-null it also writes PUBLIC "pubid" "sysid" where pubid and sysid are replaced with the value of the given arguments
        /// </summary>
        public string Pubid { get; set; }

        /// <summary>
        /// If pubid is null and sysid is non-null it writes SYSTEM "sysid" where sysid is replaced with the value of this argument.
        /// </summary>
        public string Sysid { get; set; }

        /// <summary>
        /// If non-null it writes [subset] where subset is replaced with the value of this argument.
        /// </summary>
        public string Subset { get; set; }

        /// <summary>
        /// Instantiates an instance of the SerializeAsXmlAttribute class.
        /// </summary>
        public SerializeAsXmlAttribute()
        {
            Encoding = CharEncoding.Utf8;
            Indent = true;
            IndentChars = "\t";
        }

        /// <summary>
        /// Serializes the specified object into the request stream.
        /// </summary>
        public override void Serialize(object obj, WebRequest request)
        {
            request.ContentType = "text/xml";

            // stream is disposed by the reader
            using (var writer = XmlWriter.Create(request.GetRequestStream(), GetSettings(obj)))
            {
                Save(obj, writer);
            }
        }

        /// <summary>
        /// Gets setting for the XmlWriter.
        /// </summary>
        protected virtual XmlWriterSettings GetSettings(object obj)
        {
            return new XmlWriterSettings
            {
                Encoding = GetEncoding(obj),
                Indent = Indent,
                IndentChars = IndentChars
            };
        }

        /// <summary>
        /// Gets character encoding for XmlWriter settings.
        /// </summary>
        protected virtual Encoding GetEncoding(object obj)
        {
            switch (Encoding)
            {
                case CharEncoding.Ascii:
                    return System.Text.Encoding.ASCII;
                case CharEncoding.Utf16:
                    return System.Text.Encoding.Unicode;
                case CharEncoding.Utf32:
                    return System.Text.Encoding.UTF32;
                default:
                    return System.Text.Encoding.UTF8;
            }
        }

        /// <summary>
        /// Serializes the object using the specified XmlWriter.
        /// If DOCTYPE definition is configured it will be written as well.
        /// </summary>
        protected virtual void Save(object obj, XmlWriter writer)
        {
            TryWriteDocType(obj, writer);

            new XmlSerializer(obj.GetType()).Serialize(writer, obj, GetNamespaces(obj));
        }

        /// <summary>
        /// Writes the DOCTYPE definition if it's configured.
        /// </summary>
        protected virtual void TryWriteDocType(object obj, XmlWriter writer)
        {
            if (string.IsNullOrWhiteSpace(DocTypeName) && string.IsNullOrWhiteSpace(DocTypeName)
                && string.IsNullOrWhiteSpace(DocTypeName) && string.IsNullOrWhiteSpace(DocTypeName))
            {
                return;
            }

            writer.WriteDocType(DocTypeName, Pubid, Sysid, Subset);
        }

        /// <summary>
        /// Gets namespace definitions for the XmlSerializer.
        /// The default behaviour prevents definitions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"
        /// to be written.
        /// </summary>
        protected virtual XmlSerializerNamespaces GetNamespaces(object obj)
        {
            var ns = new XmlSerializerNamespaces();
            
            ns.Add("", "");

            return ns;
        }
    }
}
