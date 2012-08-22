using System.IO;
using System.IO.Compression;

namespace DocaLabs.Http.Client.ContentEncoding
{
    /// <summary>
    /// Defines deflate encoder for the http content.
    /// </summary>
    public class DefaultInfatingEncoder : IEncode
    {
        /// <summary>
        /// Returns DeflateStream in compress mode.
        /// </summary>
        public Stream Encode(Stream stream)
        {
            return new DeflateStream(stream, CompressionMode.Compress);
        }
    }
}