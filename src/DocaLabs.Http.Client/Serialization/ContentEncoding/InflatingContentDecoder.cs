using System.IO;
using System.IO.Compression;

namespace DocaLabs.Http.Client.Serialization.ContentEncoding
{
    /// <summary>
    /// Defines deflate decoder for the http content.
    /// </summary>
    public class InflatingContentDecoder : IDecodeContent
    {
        /// <summary>
        /// Returns DeflateStream in decompress mode.
        /// </summary>
        public Stream GetDecompressionStream(Stream stream)
        {
            return new DeflateStream(stream, CompressionMode.Decompress);
        }
    }
}