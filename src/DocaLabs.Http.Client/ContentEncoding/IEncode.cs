using System.IO;

namespace DocaLabs.Http.Client.ContentEncoding
{
    /// <summary>
    /// Defines encoder for the http content.
    /// </summary>
    public interface IEncode
    {
        /// <summary>
        /// Encodes a stream and returns stream with already encoded data or stream that can encode them.
        /// </summary>
        Stream Encode(Stream stream);
    }
}