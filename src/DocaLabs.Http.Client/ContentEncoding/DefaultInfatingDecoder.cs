using System.IO;
using System.IO.Compression;

namespace DocaLabs.Http.Client.ContentEncoding
{
    /// <summary>
    /// Defines deflate decoder for the http content.
    /// </summary>
    public class DefaultInfatingDecoder : IDecode
    {
        /// <summary>
        /// Returns DeflateStream in decompress mode.
        /// </summary>
        public Stream Decode(Stream stream)
        {
            return new DeflateStream(stream, CompressionMode.Decompress);
        }
    }
}