using System;
using System.Collections.Generic;
using System.Text;

namespace DocaLabs.Utils
{
    /// <summary>
    /// Extension methods for strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns substring without exception if the start + length greater than the string length.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="start">The zero-based starting character position of a substring in this instance.</param>
        /// <param name="length">The number of characters to be copied.</param>
        /// <returns>
        /// A string that is equivalent to the substring of length length that begins at startIndex in this instance,
        /// or System.String.Empty if startIndex is equal to the length of this instance and length is zero.
        /// </returns>
        public static string SafeSubstring(this string value, int start, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length", Resources.Text.length_cannot_be_less_than_zero);

            var maxLength = value.Length;
            if(start < 0 || start > maxLength)
                throw new ArgumentOutOfRangeException("start");

            if (length == 0)
                return string.Empty;

            if ((start + length) > maxLength)
                length = maxLength - start;

            return length <= 0 ? string.Empty : value.Substring(start, length);
        }
    }
}
