using System;
using System.IO;
using System.Text;

namespace DocaLabs.Utils
{
    /// <summary>
    /// Stream extensions
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Copies range of bytes from one stream to the current position in another.
        /// </summary>
        /// <param name="in">Source stream.</param>
        /// <param name="offset">Offset in source stream where to start copy.</param>
        /// <param name="range">Length of the block to copy.</param>
        /// <param name="out">Destination stream.</param>
        /// <param name="bufferSize">Buffer size.</param>
        /// <returns>Number of actually copied bytes.</returns>
        public static bool CopyRangeTo(this Stream @in, long offset, int range, Stream @out, int bufferSize = 64*1024)
        {
            if(@in == null)
                throw new ArgumentNullException("in");

            if(@out == null)
                throw new ArgumentNullException("out");

            if(range <= 0)
                return false;

            if (bufferSize <= 0)
                bufferSize = 64*1024;

            @in.Seek(offset, SeekOrigin.Begin);

            var buffer = new byte[Math.Min(bufferSize, range)];

            var len = @in.Read(buffer, 0, buffer.Length);

            var written = false;

            while (len > 0)
            {
                @out.Write(buffer, 0, len);

                written = true;

                range -= len;

                if(range <= 0)
                    break;

                len = @in.Read(buffer, 0, Math.Min(buffer.Length, range));
            }

            return written;
        }

        /// <summary>
        /// Copies string to stream using UTF8 encoding.
        /// </summary>
        /// <param name="data">Input string.</param>
        /// <returns>Stream containing the input string.</returns>
        public static Stream StringToStream(this string data)
        {
            return data != null
                       ? new MemoryStream(Encoding.UTF8.GetBytes(data))
                       : null;
        }

        /// <summary>
        /// Copies stream to string using UTF8 encoding.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <returns>Converted string.</returns>
        public static string StreamToString(this Stream stream)
        {
            var memoryStream = stream as MemoryStream;

            if(memoryStream != null)
            {
                try
                {
                    return Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                }
                catch (UnauthorizedAccessException)
                {
                }    
            }

            return stream != null
                       ? ByteArrayToString(StreamToByteArray(stream))
                       : null;
        }

        /// <summary>
        /// Copies byte array to string using UTF8 encoding.
        /// </summary>
        /// <param name="data">Source byte array.</param>
        /// <returns>Resulting string.</returns>
        public static string ByteArrayToString(this byte[] data)
        {
            return data != null
                       ? Encoding.UTF8.GetString(data)
                       : null;
        }

        /// <summary>
        /// Copies string to byte array using UTF8 encoding.
        /// </summary>
        /// <param name="data">Source string.</param>
        /// <returns>Resulting byte array.</returns>
        public static byte[] StringToByteArray(this string data)
        {
            return data != null
                       ? Encoding.UTF8.GetBytes(data)
                       : null;
        }

        /// <summary>
        /// Copies content of the stream from the current position to the byte array.
        /// </summary>
        /// <param name="stream">Source stream.</param>
        /// <param name="prefix">Optional prefix to be copied.</param>
        /// <returns>Resulting byte array.</returns>
        public static byte[] StreamToByteArray(this Stream stream, byte [] prefix = null)
        {
            if (stream == null)
                return null;

            if (!stream.CanSeek)
            {   
                // most likely the stream won't support data.Length
                using (var memoryStream = new MemoryStream(8192))
                {
                    if (prefix != null)
                        memoryStream.Write(prefix, 0, prefix.Length);

                    stream.CopyTo(memoryStream);

                    return memoryStream.ToArray();
                }
            }

            var effectiveLength = stream.Length - stream.Position;

            if (prefix == null)
            {
                var data = new byte[effectiveLength];

                stream.Read(data, 0, (int)effectiveLength);

                return data;
            }
            else
            {
                var data = new byte[prefix.Length + effectiveLength];

                prefix.CopyTo(data, 0);

                stream.Read(data, prefix.Length, (int)effectiveLength);

                return data;
            }
        }

        /// <summary>
        /// Copies byte array to stream.
        /// </summary>
        /// <param name="array">Source byte array.</param>
        /// <param name="index">Index in the byte array which to start from.</param>
        /// <param name="count">Number of byte to copy, -1 means until the end.</param>
        /// <returns>Resulting stream.</returns>
        public static Stream ByteArrayToStream(this byte[] array, int index = 0, int count = -1)
        {
            if (count < 0)
                count = array.Length - index;

            return new MemoryStream(array, index, count);
        }

        /// <summary>
        /// Safely (checks that the stream supports Seek operation) set the current position in the stream to its beginning.
        /// </summary>
        /// <param name="stream">Stream which position to reset.</param>
        /// <returns>The stream itself.</returns>
        public static Stream ResetToBegining(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
