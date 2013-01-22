﻿using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DocaLabs.Http.Client.Serialization.ContentEncoding;

namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Serializes a given object into the web request in xml format.
    /// The class uses XmlSerializer for serialization.
    /// </summary>
    public class SerializeAsXmlAttribute : RequestSerializationAttribute
    {
        /// <summary>
        /// Gets or sets the content encoding, if RequestContentEncoding blank or null no encoding is done.
        /// The encoder is supplied by ContentEncoderFactory.
        /// </summary>
        public string RequestContentEncoding { get; set; }

        /// <summary>
        /// Gets or sets the type of text encoding to be used for Xml serialization.
        /// The default value is UTF-8.
        /// </summary>
        public Encoding Encoding { get; set; }

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
            Encoding = Encoding.UTF8;
            Indent = true;
            IndentChars = "\t";
        }

        /// <summary>
        /// Serializes the specified object into the request stream.
        /// </summary>
        public override void Serialize(object obj, WebRequest request)
        {
            request.ContentType = "text/xml";

            if (string.IsNullOrWhiteSpace(RequestContentEncoding))
                Write(obj, request);
            else
                EncodeAndWrite(obj, request);
        }

        XmlWriterSettings GetSettings()
        {
            return new XmlWriterSettings
            {
                Encoding = Encoding,
                Indent = Indent,
                IndentChars = IndentChars
            };
        }

        void Write(object obj, WebRequest request)
        {
            // stream is disposed by the reader
            using (var writer = XmlWriter.Create(request.GetRequestStream(), GetSettings()))
            {
                TryWriteDocType(writer);

                new XmlSerializer(obj.GetType()).Serialize(writer, obj, GetNamespaces());
            }
        }

        void EncodeAndWrite(object obj, WebRequest request)
        {
            request.Headers.Add(string.Format("content-encoding: {0}", RequestContentEncoding));

            // stream is disposed by the reader
            var dataStream = new MemoryStream();

            using (var writer = XmlWriter.Create(dataStream, GetSettings()))
            {
                TryWriteDocType(writer);

                new XmlSerializer(obj.GetType()).Serialize(writer, obj, GetNamespaces());

                dataStream.Seek(0, SeekOrigin.Begin);

                using (var requestStream = request.GetRequestStream())
                using (var compressionStream = ContentEncoderFactory.Get(RequestContentEncoding).GetCompressionStream(dataStream))
                {
                    compressionStream.CopyTo(requestStream);
                }
            }
        }

        void TryWriteDocType(XmlWriter writer)
        {
            if (string.IsNullOrWhiteSpace(DocTypeName) && string.IsNullOrWhiteSpace(DocTypeName)
                && string.IsNullOrWhiteSpace(DocTypeName) && string.IsNullOrWhiteSpace(DocTypeName))
            {
                return;
            }

            writer.WriteDocType(DocTypeName, Pubid, Sysid, Subset);
        }

        static XmlSerializerNamespaces GetNamespaces()
        {
            var ns = new XmlSerializerNamespaces();
            
            ns.Add("", "");

            return ns;
        }
    }
}
