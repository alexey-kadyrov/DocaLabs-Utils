using System.IO;
using System.IO.Compression;

namespace DocaLabs.Http.Client.ContentEncoding
{
    /// <summary>
    /// Defines gzip decoder for the http content.
    /// </summary>
    public class DefaultGZipDecoder : IDecode
    {
        /// <summary>
        /// Returns GZipStream in decompress mode.
        /// </summary>
        public Stream Decode(Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Decompress);
        }
    }
}