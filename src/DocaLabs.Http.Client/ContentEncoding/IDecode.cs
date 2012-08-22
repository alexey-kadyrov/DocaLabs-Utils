using System.IO;

namespace DocaLabs.Http.Client.ContentEncoding
{
    /// <summary>
    /// Defines decoder for the http content.
    /// </summary>
    public interface IDecode
    {
        /// <summary>
        /// Decodes a stream and returns stream with already decoded data or stream that can decode them.
        /// </summary>
        Stream Decode(Stream stream);
    }
}