namespace DocaLabs.Http.Client.ContentEncoding
{
    /// <summary>
    /// Defines a factory that can provide content encoders/decoders for a specified content encoding.
    /// </summary>
    public interface IEncodingProvider
    {
        /// <summary>
        /// Gets an encoder (compressor) for a specified content encoding.
        /// If the encoding is not supported than NotSupportedException is thrown.
        /// </summary>
        IEncode GetEncoder(string encoding);

        /// <summary>
        /// Gets a decoder (decompressor) for a specified content encoding.
        /// If the encoding is not supported than NotSupportedException is thrown.
        /// </summary>
        IDecode GetDecoder(string encoding);
    }
}
