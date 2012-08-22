using System;
using NUnit.Framework;

namespace DocaLabs.Utils.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        #region SafeSubstring

        [Test]
        public void SafeSubstringReturnsSubstring()
        {
            Assert.AreEqual("0123456789", "0123456789".SafeSubstring(0, 10));
            Assert.AreEqual("0123456789", "0123456789".SafeSubstring(0, 12));
            Assert.AreEqual("34", "0123456789".SafeSubstring(3, 2));
            Assert.AreEqual("3456", "0123456789".SafeSubstring(3, 4));
            Assert.AreEqual("345678", "0123456789".SafeSubstring(3, 6));
            Assert.AreEqual("3456789", "0123456789".SafeSubstring(3, 7));
            Assert.AreEqual("3456789", "0123456789".SafeSubstring(3, 8));
            Assert.AreEqual("3456789", "0123456789".SafeSubstring(3, 9));
        }

        [Test]
        public void SafeSubstringReturnsEmptySubstring()
        {
            Assert.AreEqual("", "".SafeSubstring(0, 0));
            Assert.AreEqual("", "".SafeSubstring(0, 1));
            Assert.AreEqual("", "".SafeSubstring(0, 2));
            Assert.AreEqual("", "0123456789".SafeSubstring(3, 0));
            Assert.AreEqual("", "0123456789".SafeSubstring(9, 0));
            Assert.AreEqual("", "0123456789".SafeSubstring(10, 0));
            Assert.AreEqual("", "0123456789".SafeSubstring(10, 1));
            Assert.AreEqual("", "0123456789".SafeSubstring(10, 2));
        }

        [Test]
        public void SafeSubstringThrowsArgumentNullExceptionThenStringIsNull()
        {
            var exception = Assert.Catch<ArgumentNullException>(() => ((string)null).SafeSubstring(0, 0));

            Assert.AreEqual("value", exception.ParamName);
        }

        [Test]
        public void SafeSubstringThrowsArgumentOutOfRangeExceptionThenLengthIsLessThanZero()
        {
            var exception = Assert.Catch<ArgumentOutOfRangeException>(() => "0123456789".SafeSubstring(0, -1));

            Assert.AreEqual("length", exception.ParamName);
        }

        [Test]
        public void SafeSubstringThrowsArgumentOutOfRangeExceptionThenStartIsLessThanZero()
        {
            var exception = Assert.Catch<ArgumentOutOfRangeException>(() => "0123456789".SafeSubstring(-2, 2));

            Assert.AreEqual("start", exception.ParamName);
        }

        [Test]
        public void SafeSubstringThrowsArgumentOutOfRangeExceptionThenStartIsbeoyndStringLength()
        {
            var exception = Assert.Catch<ArgumentOutOfRangeException>(() => "0123456789".SafeSubstring(11, 2));

            Assert.AreEqual("start", exception.ParamName);
        }

        #endregion SafeSubstring

        #region EnumerableToString

        [Test]
        [TestCase("hello", "hello")]
        [TestCase(";", ";")]
        [TestCase("hello;world", "hello;world")]
        [TestCase(";hello;world", ";hello;world")]
        [TestCase(";hello;world;", ";hello;world;")]
        [TestCase("hello;world;and;or;", "hello;world;and;or;")]
        public void ConvertsArrayToString(string source, string expectedResult)
        {
            var result = source.Split(new [] { ';' }, StringSplitOptions.None).EnumerableToString();

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ArrayToStringReturnsNullForZeroSizeArray()
        {
            var result = new string[0].EnumerableToString();

            Assert.IsNull(result);
        }

        [Test]
        public void ArrayToStringReturnsNullForNullArray()
        {
            var result = StringExtensions.EnumerableToString(null);

            Assert.IsNull(result);
        }

        #endregion EnumerableToString
    }
}
