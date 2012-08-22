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

        /// <summary>
        /// Converts collection of strings to semicolon separated string.
        /// </summary>
        /// <param name="in">Input collection of strings.</param>
        /// <returns>Semicolon separated string consisting of input string values.</returns>
        public static string EnumerableToString(this IEnumerable<string> @in)
        {
            if (@in == null)
                return null;

            var nameBuilder = new StringBuilder(512);

            var first = true;

            foreach (var value in @in)
            {
                if (first)
                    first = false;
                else
                    nameBuilder.Append(";");

                nameBuilder.Append(value);
            }

            return nameBuilder.Length == 0 ? null: nameBuilder.ToString();
        }
    }
}
