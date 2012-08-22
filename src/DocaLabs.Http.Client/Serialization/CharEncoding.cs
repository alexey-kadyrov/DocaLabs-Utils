namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Defines constants describing character encodings.
    /// </summary>
    public enum CharEncoding
    {
        /// <summary>
        /// Unicode characters using the UTF-8 encoding. This encoding supports all Unicode character values. Code page 65001.
        /// </summary>
        Utf8 = 0,

        /// <summary>
        /// Unicode characters as single 7-bit ASCII characters. This encoding only supports character values between U+0000 and U+007F. Code page 20127.
        /// </summary>
        Ascii = 1, 

        /// <summary>
        /// Unicode characters using the UTF-16 encoding. Both little endian and big endian byte orders are supported.
        /// </summary>
        Utf16 = 3,

        /// <summary>
        /// Unicode characters using the UTF-32 encoding. Both little endian (code page 12000) and big endian (code page 12001) byte orders are supported.
        /// </summary>
        Utf32 = 4
    }
}
