using System;
using DocaLabs.Http.Client.Resources;

namespace DocaLabs.Http.Client.ContentEncoding
{
    /// <summary>
    /// Defines a factory which uses standard .Net GZipStream and DeflateStream and can provide content encoders/decoders for a specified content encoding.
    /// </summary>
    public class DefaultEncodingProvider : IEncodingProvider
    {
        /// <summary>
        /// Gets an encoder (compressor) for a specified content encoding.
        /// It supports only gzip/x-gzip/deflate encodings, for other it will throw NotSupportedException.
        /// </summary>
        public virtual IEncode GetEncoder(string encoding)
        {
            if (string.IsNullOrWhiteSpace(encoding))
                throw new ArgumentNullException("encoding");

            if (IsGZip(encoding))
                return new DefaultGZipEncoder();

            if (IsDeflate(encoding))
                return new DefaultInfatingEncoder();

            throw new NotSupportedException(string.Format(Text.compression_format_is_not_suppoerted, encoding));
        }

        /// <summary>
        /// Gets a decoder for a specified content encoding.
        /// It supports only gzip/x-gzip/deflate encodings, for other it will throw NotSupportedException.
        /// </summary>
        public virtual IDecode GetDecoder(string encoding)
        {
            if (string.IsNullOrWhiteSpace(encoding))
                throw new ArgumentNullException("encoding");

            if (IsGZip(encoding))
                return new DefaultGZipDecoder();

            if (IsDeflate(encoding))
                return new DefaultInfatingDecoder();

            throw new NotSupportedException(string.Format(Text.compression_format_is_not_suppoerted, encoding));
        }

        /// <summary>
        /// Returns true if the encoding is gzip or x-gzip, otherwise false.
        /// </summary>
        public bool IsGZip(string encoding)
        {
            return string.Compare(encoding, "gzip", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(encoding, "x-gzip", StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns true if the encoding is deflate, otherwise false.
        /// </summary>
        public static bool IsDeflate(string encoding)
        {
            return string.Compare(encoding, "deflate", StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
