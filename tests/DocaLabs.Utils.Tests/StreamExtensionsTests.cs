using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace DocaLabs.Utils.Tests
{
    [TestFixture]
    public class StreamExtensionsTests
    {
        const string Data = // around 12K
            " Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris nisi neque, euismod ac scelerisque nec, congue at mauris. Aliquam sed tempus ipsum. Nam aliquam, augue in tempus tristique, elit tellus dignissim neque, eu porta leo leo nec tortor. Nulla magna enim, volutpat nec aliquet eget, varius at metus. Aenean tempor enim ut diam condimentum aliquet. Nulla a urna massa, eget auctor ante. Quisque ut nisi urna. Praesent tristique tellus ac tellus imperdiet scelerisque. In hac habitasse platea dictumst. Vivamus aliquam, elit id elementum imperdiet, massa mi euismod nibh, eu volutpat purus ligula at ante. Vestibulum felis quam, iaculis quis dignissim sed, scelerisque sed felis. Nunc et nunc sapien. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Etiam accumsan, mi sit amet tempor luctus, erat tellus iaculis nisi, quis posuere purus mauris id justo. Maecenas congue volutpat massa sagittis venenatis. Suspendisse sed elit sit amet odio accumsan tempor quis et lacus. Maecenas risus risus, tempus eget pulvinar vel, gravida at lacus. Vestibulum tincidunt ante vel lorem condimentum nec scelerisque risus vehicula. Etiam ut leo vitae nulla porta sodales sed vitae turpis." +
            " Curabitur tincidunt ultrices nulla quis aliquet. Aenean accumsan eleifend erat, nec vestibulum arcu volutpat in. Curabitur adipiscing turpis nec eros porttitor elementum commodo et libero. Aliquam vitae nisi diam, in laoreet metus. Nunc scelerisque cursus diam. Praesent diam tellus, commodo vel pretium id, ultrices at arcu. Ut tristique orci sit amet augue imperdiet cursus. Duis eu elit mauris. Proin eu nisl a velit vestibulum mattis ac quis nunc. Nulla vel nunc erat, quis faucibus sem. Donec dignissim scelerisque bibendum." +
            " Phasellus sodales convallis mauris, id tempor urna consectetur in. Praesent ac lorem est, vel pulvinar dui. Sed sodales consectetur suscipit. Nullam sem neque, rutrum vitae faucibus a, dapibus sit amet metus. Aenean congue, urna id accumsan dictum, enim elit egestas justo, vitae pellentesque magna risus eget nisi. Proin mollis tellus id nunc scelerisque vel iaculis nunc volutpat. Fusce sed augue est. Sed imperdiet adipiscing ullamcorper. Quisque id posuere nisl. Duis laoreet, mauris ac lobortis placerat, nisl augue ornare nulla, at pellentesque risus velit ut nulla. Nam nibh neque, tincidunt in ultrices eget, blandit in mi. Suspendisse potenti. Curabitur tristique hendrerit lectus. Nullam in ligula ac arcu consequat congue in quis lectus." +
            " Praesent consequat vestibulum leo, sit amet interdum leo scelerisque et. Maecenas lobortis placerat nunc id sollicitudin. Curabitur vel dignissim lorem. Nulla facilisi. Donec commodo tellus vel nisi vestibulum congue. Praesent eget magna nec nibh semper faucibus. Ut congue, turpis sit amet lacinia pretium, turpis velit condimentum mi, non auctor nibh velit vitae sem. Curabitur iaculis vulputate placerat. Quisque tincidunt fermentum volutpat. Morbi ullamcorper lectus ut odio ornare suscipit. Sed aliquam bibendum blandit." +
            " Pellentesque eget lectus neque, eget tincidunt arcu. Nunc dictum turpis eget arcu auctor vel placerat erat lacinia. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Mauris tempus nisl quis ligula scelerisque in iaculis tellus ultrices. Proin interdum laoreet aliquet. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Etiam lacinia turpis non felis pellentesque laoreet. Proin ut urna eros, vel tincidunt lectus. Integer ornare iaculis purus, eu mollis diam tincidunt non. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Morbi nec dolor non nulla malesuada laoreet. Phasellus eleifend suscipit ullamcorper. Quisque eu urna felis, in egestas arcu. Pellentesque molestie dignissim metus, in condimentum elit adipiscing id." +
            " Quisque lacus odio, fringilla ut elementum ut, ultricies sit amet diam. Vestibulum commodo nisi quis lorem porttitor congue porta libero ultricies. Sed facilisis sagittis dictum. Curabitur viverra viverra neque a placerat. Donec sagittis, massa et gravida pellentesque, dui ipsum accumsan felis, eu rhoncus enim arcu sit amet ante. In molestie, est sed ullamcorper egestas, lorem nisi tempus mauris, volutpat laoreet erat orci non ante. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Nulla turpis urna, consectetur vel placerat rutrum, rutrum eget metus. Quisque scelerisque est quis odio venenatis sed porta nunc mattis. In neque sem, rutrum a mollis vel, suscipit nec est. Integer egestas leo a lacus molestie non tincidunt est ullamcorper. Curabitur dignissim lorem eget nisi fringilla vel tristique tellus dignissim. Maecenas id consectetur sapien. Suspendisse potenti. Donec sagittis dapibus nulla, sed tempus eros imperdiet id. Aliquam sem metus, facilisis sed posuere eget, lobortis vitae lorem. Sed aliquet sem nibh, ac dignissim dolor. In a tellus vel urna posuere molestie. Aliquam a diam diam, viverra rhoncus augue." +
            " Sed auctor massa a leo adipiscing sed ultrices mi consectetur. Praesent condimentum semper mauris, at commodo erat condimentum sed. Cras nec augue et nulla ornare tincidunt suscipit in elit. Sed turpis arcu, consectetur at elementum vel, volutpat ut leo. Donec lorem nulla, tincidunt sit amet molestie ac, volutpat non orci. Nulla nec diam ut turpis malesuada elementum. Nullam quis adipiscing nibh. Nam mi nunc, tempus ut fermentum ut, adipiscing et ante. Vivamus feugiat mattis ante eget condimentum. Cras molestie pulvinar lobortis. Duis sit amet lorem quis velit facilisis venenatis. Fusce sit amet nulla eget mauris pharetra euismod sed in enim. Aliquam vitae tortor id purus eleifend dignissim." +
            " Fusce quis quam dolor. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Mauris pulvinar lacus ut dolor laoreet mollis. Vivamus non eros sit amet neque aliquam condimentum. Integer semper tellus non velit ultrices tincidunt eget et magna. Fusce eu cursus ligula. Vestibulum ac mauris a magna vehicula pharetra. Suspendisse nec lorem tellus. Phasellus ut magna et orci varius tristique sit amet nec purus. Donec in sapien ut ligula bibendum rhoncus at sit amet est." +
            " Aenean blandit porta tortor, at condimentum nunc rutrum at. Phasellus mollis enim ac eros adipiscing ac tristique nisi tempor. Sed dui elit, feugiat eu convallis nec, consequat eleifend nulla. Vestibulum vel nisl neque, id pretium erat. Etiam cursus nisi vitae metus aliquet convallis tincidunt mi semper. Sed id pellentesque mi. Quisque viverra malesuada eleifend. Morbi pellentesque sollicitudin nulla, ut tristique quam euismod quis." +
            " Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris nisi neque, euismod ac scelerisque nec, congue at mauris. Aliquam sed tempus ipsum. Nam aliquam, augue in tempus tristique, elit tellus dignissim neque, eu porta leo leo nec tortor. Nulla magna enim, volutpat nec aliquet eget, varius at metus. Aenean tempor enim ut diam condimentum aliquet. Nulla a urna massa, eget auctor ante. Quisque ut nisi urna. Praesent tristique tellus ac tellus imperdiet scelerisque. In hac habitasse platea dictumst. Vivamus aliquam, elit id elementum imperdiet, massa mi euismod nibh, eu volutpat purus ligula at ante. Vestibulum felis quam, iaculis quis dignissim sed, scelerisque sed felis. Nunc et nunc sapien. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Etiam accumsan, mi sit amet tempor luctus, erat tellus iaculis nisi, quis posuere purus mauris id justo. Maecenas congue volutpat massa sagittis venenatis. Suspendisse sed elit sit amet odio accumsan tempor quis et lacus. Maecenas risus risus, tempus eget pulvinar vel, gravida at lacus. Vestibulum tincidunt ante vel lorem condimentum nec scelerisque risus vehicula. Etiam ut leo vitae nulla porta sodales sed vitae turpis." +
            " Curabitur tincidunt ultrices nulla quis aliquet. Aenean accumsan eleifend erat, nec vestibulum arcu volutpat in. Curabitur adipiscing turpis nec eros porttitor elementum commodo et libero. Aliquam vitae nisi diam, in laoreet metus. Nunc scelerisque cursus diam. Praesent diam tellus, commodo vel pretium id, ultrices at arcu. Ut tristique orci sit amet augue imperdiet cursus. Duis eu elit mauris. Proin eu nisl a velit vestibulum mattis ac quis nunc. Nulla vel nunc erat, quis faucibus sem. Donec dignissim scelerisque bibendum." +
            " Phasellus sodales convallis mauris, id tempor urna consectetur in. Praesent ac lorem est, vel pulvinar dui. Sed sodales consectetur suscipit. Nullam sem neque, rutrum vitae faucibus a, dapibus sit amet metus. Aenean congue, urna id accumsan dictum, enim elit egestas justo, vitae pellentesque magna risus eget nisi. Proin mollis tellus id nunc scelerisque vel iaculis nunc volutpat. Fusce sed augue est. Sed imperdiet adipiscing ullamcorper. Quisque id posuere nisl. Duis laoreet, mauris ac lobortis placerat, nisl augue ornare nulla, at pellentesque risus velit ut nulla. Nam nibh neque, tincidunt in ultrices eget, blandit in mi. Suspendisse potenti. Curabitur tristique hendrerit lectus. Nullam in ligula ac arcu consequat congue in quis lectus." +
            " Praesent consequat vestibulum leo, sit amet interdum leo scelerisque et. Maecenas lobortis placerat nunc id sollicitudin. Curabitur vel dignissim lorem. Nulla facilisi. Donec commodo tellus vel nisi vestibulum congue. Praesent eget magna nec nibh semper faucibus. Ut congue, turpis sit amet lacinia pretium, turpis velit condimentum mi, non auctor nibh velit vitae sem. Curabitur iaculis vulputate placerat. Quisque tincidunt fermentum volutpat. Morbi ullamcorper lectus ut odio ornare suscipit. Sed aliquam bibendum blandit." +
            " Pellentesque eget lectus neque, eget tincidunt arcu. Nunc dictum turpis eget arcu auctor vel placerat erat lacinia. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Mauris tempus nisl quis ligula scelerisque in iaculis tellus ultrices. Proin interdum laoreet aliquet. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Etiam lacinia turpis non felis pellentesque laoreet. Proin ut urna eros, vel tincidunt lectus. Integer ornare iaculis purus, eu mollis diam tincidunt non. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Morbi nec dolor non nulla malesuada laoreet. Phasellus eleifend suscipit ullamcorper. Quisque eu urna felis, in egestas arcu. Pellentesque molestie dignissim metus, in condimentum elit adipiscing id." +
            " Quisque lacus odio, fringilla ut elementum ut, ultricies sit amet diam. Vestibulum commodo nisi quis lorem porttitor congue porta libero ultricies. Sed facilisis sagittis dictum. Curabitur viverra viverra neque a placerat. Donec sagittis, massa et gravida pellentesque, dui ipsum accumsan felis, eu rhoncus enim arcu sit amet ante. In molestie, est sed ullamcorper egestas, lorem nisi tempus mauris, volutpat laoreet erat orci non ante. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Nulla turpis urna, consectetur vel placerat rutrum, rutrum eget metus. Quisque scelerisque est quis odio venenatis sed porta nunc mattis. In neque sem, rutrum a mollis vel, suscipit nec est. Integer egestas leo a lacus molestie non tincidunt est ullamcorper. Curabitur dignissim lorem eget nisi fringilla vel tristique tellus dignissim. Maecenas id consectetur sapien. Suspendisse potenti. Donec sagittis dapibus nulla, sed tempus eros imperdiet id. Aliquam sem metus, facilisis sed posuere eget, lobortis vitae lorem. Sed aliquet sem nibh, ac dignissim dolor. In a tellus vel urna posuere molestie. Aliquam a diam diam, viverra rhoncus augue." +
            " Sed auctor massa a leo adipiscing sed ultrices mi consectetur. Praesent condimentum semper mauris, at commodo erat condimentum sed. Cras nec augue et nulla ornare tincidunt suscipit in elit. Sed turpis arcu, consectetur at elementum vel, volutpat ut leo. Donec lorem nulla, tincidunt sit amet molestie ac, volutpat non orci. Nulla nec diam ut turpis malesuada elementum. Nullam quis adipiscing nibh. Nam mi nunc, tempus ut fermentum ut, adipiscing et ante. Vivamus feugiat mattis ante eget condimentum. Cras molestie pulvinar lobortis. Duis sit amet lorem quis velit facilisis venenatis. Fusce sit amet nulla eget mauris pharetra euismod sed in enim. Aliquam vitae tortor id purus eleifend dignissim." +
            " Fusce quis quam dolor. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Mauris pulvinar lacus ut dolor laoreet mollis. Vivamus non eros sit amet neque aliquam condimentum. Integer semper tellus non velit ultrices tincidunt eget et magna. Fusce eu cursus ligula. Vestibulum ac mauris a magna vehicula pharetra. Suspendisse nec lorem tellus. Phasellus ut magna et orci varius tristique sit amet nec purus. Donec in sapien ut ligula bibendum rhoncus at sit amet est." +
            " Aenean blandit porta tortor, at condimentum nunc rutrum at. Phasellus mollis enim ac eros adipiscing ac tristique nisi tempor. Sed dui elit, feugiat eu convallis nec, consequat eleifend nulla. Vestibulum vel nisl neque, id pretium erat. Etiam cursus nisi vitae metus aliquet convallis tincidunt mi semper. Sed id pellentesque mi. Quisque viverra malesuada eleifend. Morbi pellentesque sollicitudin nulla, ut tristique quam euismod quis.";

        class NonSeekingStream : Stream
        {
            Stream OriginalStream { get; set; }

            public NonSeekingStream(Stream originalStream)
            {
                OriginalStream = originalStream;
            }

            public override void Flush()
            {
                OriginalStream.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return OriginalStream.Read(buffer, offset, count);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                OriginalStream.Write(buffer, offset, count);
            }

            public override bool CanRead
            {
                get { return OriginalStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return OriginalStream.CanWrite; }
            }

            public override long Length
            {
                get { return OriginalStream.Length; }
            }

            public override long Position
            {
                get { return OriginalStream.Position; }
                set { throw new NotSupportedException(); }
            }

            public override void Close()
            {
                if(OriginalStream != null)
                    OriginalStream.Close();

                base.Close();
            }
        }

        [Test]
        public void CopyRangeToCopiesMidRangeWhenTheRangeIsShorterThanBufferSize()
        {
            var random = new Random();
            var dataIn = new byte[673];
            random.NextBytes(dataIn);

            using (var source = dataIn.ByteArrayToStream())
            using (var target = new MemoryStream())
            {
                var result = source.CopyRangeTo(27, 459, target, 1024);

                var dataOut = target.ResetToBegining().StreamToByteArray();

                Assert.IsTrue(result);
                CollectionAssert.AreEqual(dataIn.Skip(27).Take(459).ToArray(), dataOut);
            }
        }

        [Test]
        public void CopyRangeToThrowsArgumentNulLExceptionForNullInputStream()
        {
            var exception = Assert.Catch<ArgumentNullException>(() => ((Stream)null).CopyRangeTo(0, 0, new MemoryStream()));
            
            Assert.AreEqual("in", exception.ParamName);
        }

        [Test]
        public void CopyRangeToThrowsArgumentNulLExceptionForNullTargetStream()
        {
            var exception = Assert.Catch<ArgumentNullException>(() => new MemoryStream().CopyRangeTo(0, 0, null));

            Assert.AreEqual("out", exception.ParamName);
        }

        [Test]
        public void CopyRangeToCopiesLastRangeWhenTheRangeIsShorterThanBufferSize()
        {
            var random = new Random();
            var dataIn = new byte[673];
            random.NextBytes(dataIn);

            using (var source = dataIn.ByteArrayToStream())
            using (var target = new MemoryStream())
            {
                var result = source.CopyRangeTo(673 - 459, 459, target, 1024);

                var dataOut = target.ResetToBegining().StreamToByteArray();

                Assert.IsTrue(result);
                CollectionAssert.AreEqual(dataIn.Skip(673 - 459).Take(459).ToArray(), dataOut);
            }
        }

        [Test]
        public void CopyRangeToCopiesMidRangeWhenTheRangeIsEqualToBufferSize()
        {
            var random = new Random();
            var dataIn = new byte[673];
            random.NextBytes(dataIn);

            using (var source = dataIn.ByteArrayToStream())
            using (var target = new MemoryStream())
            {
                var result = source.CopyRangeTo(27, 459, target, 459);

                var dataOut = target.ResetToBegining().StreamToByteArray();

                Assert.IsTrue(result);
                CollectionAssert.AreEqual(dataIn.Skip(27).Take(459).ToArray(), dataOut);
            }
        }

        [Test]
        public void CopyRangeToCopiesLastRangeWhenTheRangeIsEqualToBufferSize()
        {
            var random = new Random();
            var dataIn = new byte[673];
            random.NextBytes(dataIn);

            using (var source = dataIn.ByteArrayToStream())
            using (var target = new MemoryStream())
            {
                var result = source.CopyRangeTo(673 - 459, 459, target, 459);

                var dataOut = target.ResetToBegining().StreamToByteArray();

                Assert.IsTrue(result);
                CollectionAssert.AreEqual(dataIn.Skip(673 - 459).Take(459).ToArray(), dataOut);
            }
        }

        [Test]
        public void CopyRangeToCopiesMidRangeWhenTheRangeIsMulipleOfBufferSize()
        {
            var random = new Random();
            var dataIn = new byte[673];
            random.NextBytes(dataIn);

            using (var source = dataIn.ByteArrayToStream())
            using (var target = new MemoryStream())
            {
                var result = source.CopyRangeTo(27, 256, target, 64);

                var dataOut = target.ResetToBegining().StreamToByteArray();

                Assert.IsTrue(result);
                CollectionAssert.AreEqual(dataIn.Skip(27).Take(256).ToArray(), dataOut);
            }
        }

        [Test]
        public void CopyRangeToCopiesLastRangeWhenTheRangeIsMulipleOfBufferSize()
        {
            var random = new Random();
            var dataIn = new byte[673];
            random.NextBytes(dataIn);

            using (var source = dataIn.ByteArrayToStream())
            using (var target = new MemoryStream())
            {
                var result = source.CopyRangeTo(673 - 256, 256, target, 64);

                var dataOut = target.ResetToBegining().StreamToByteArray();

                Assert.IsTrue(result);
                CollectionAssert.AreEqual(dataIn.Skip(673 - 256).Take(256).ToArray(), dataOut);
            }
        }

        [Test]
        public void CopyRangeToCopiesMidRange()
        {
            var random = new Random();
            var dataIn = new byte[673];
            random.NextBytes(dataIn);

            using (var source = dataIn.ByteArrayToStream())
            using (var target = new MemoryStream())
            {
                var result = source.CopyRangeTo(27, 459, target, 111);

                var dataOut = target.ResetToBegining().StreamToByteArray();

                Assert.IsTrue(result);
                CollectionAssert.AreEqual(dataIn.Skip(27).Take(459).ToArray(), dataOut);
            }
        }

        [Test]
        public void CopyRangeToCopiesLastRange()
        {
            var random = new Random();
            var dataIn = new byte[673];
            random.NextBytes(dataIn);

            using (var source = dataIn.ByteArrayToStream())
            using (var target = new MemoryStream())
            {
                var result = source.CopyRangeTo(673 - 459, 459, target, 111);

                var dataOut = target.ResetToBegining().StreamToByteArray();

                Assert.IsTrue(result);
                CollectionAssert.AreEqual(dataIn.Skip(673 - 459).Take(459).ToArray(), dataOut);
            }
        }

        [Test]
        public void CopyRangeToCopiesRangeEvenIfTheBufferSizeIsSetToZero()
        {
            var random = new Random();
            var dataIn = new byte[673];
            random.NextBytes(dataIn);

            using (var source = dataIn.ByteArrayToStream())
            using (var target = new MemoryStream())
            {
                var result = source.CopyRangeTo(27, 459, target, 0);

                var dataOut = target.ResetToBegining().StreamToByteArray();

                Assert.IsTrue(result);
                CollectionAssert.AreEqual(dataIn.Skip(27).Take(459).ToArray(), dataOut);
            }
        }

        [Test]
        public void CopyRangeToCopiesRangeEvenIfTheBufferSizeIsLessThanZero()
        {
            var random = new Random();
            var dataIn = new byte[673];
            random.NextBytes(dataIn);

            using (var source = dataIn.ByteArrayToStream())
            using (var target = new MemoryStream())
            {
                var result = source.CopyRangeTo(27, 459, target, -1);

                var dataOut = target.ResetToBegining().StreamToByteArray();

                Assert.IsTrue(result);
                CollectionAssert.AreEqual(dataIn.Skip(27).Take(459).ToArray(), dataOut);
            }
        }

        [Test]
        public void CopyRangeToReturnsFalseForZeroRangeAndDoesNotCopy()
        {
            var random = new Random();
            var dataIn = new byte[673];
            random.NextBytes(dataIn);

            using (var source = dataIn.ByteArrayToStream())
            using (var target = new MemoryStream())
            {
                var result = source.CopyRangeTo(27, 0, target, 1024);

                Assert.IsFalse(result);
                Assert.AreEqual(0, target.Length);
            }
        }

        [Test]
        public void StreamToByteArrayReturnsNullForNullStream()
        {
            Assert.IsNull(((Stream)null).StreamToByteArray());
        }

        [Test]
        public void StreamToByteArrayTramsformsWhenStreamIsAtTheBegining()
        {
            var random = new Random();
            var dataIn = new byte[1263];
            random.NextBytes(dataIn);

            using (var stream = new MemoryStream(dataIn))
            {
                var result = stream.StreamToByteArray();

                CollectionAssert.AreEqual(dataIn, result);
            }
        }

        [Test]
        public void StreamToByteArrayTramsformsWhenStreamIsAtTheBeginingForNonSeekingStream()
        {
            var random = new Random();
            var dataIn = new byte[1263];
            random.NextBytes(dataIn);

            using (var stream = new NonSeekingStream(new MemoryStream(dataIn)))
            {
                var result = stream.StreamToByteArray();

                CollectionAssert.AreEqual(dataIn, result);
            }
        }

        [Test]
        public void StreamToByteArrayTramsformsWhenStreamIsOffset()
        {
            var random = new Random();
            var dataIn = new byte[1263];
            random.NextBytes(dataIn);

            using (var stream = new MemoryStream(dataIn))
            {
                stream.Seek(172, SeekOrigin.Begin);

                var result = stream.StreamToByteArray();

                CollectionAssert.AreEqual(dataIn.Skip(172).ToArray(), result);
            }
        }

        [Test]
        public void StreamToByteArrayTramsformsWhenStreamIsOffsetForNonSeekingStream()
        {
            var random = new Random();
            var dataIn = new byte[1263];
            random.NextBytes(dataIn);

            using (var stream = new NonSeekingStream(new MemoryStream(dataIn)))
            {
                var buffer = new byte[172];
                stream.Read(buffer, 0, 172);

                var result = stream.StreamToByteArray();

                CollectionAssert.AreEqual(dataIn.Skip(172).ToArray(), result);
            }
        }

        [Test]
        public void RoundtripStringToStreamThenStreamToByteArrayThenByteArrayToString()
        {
            using (var stream = Data.StringToStream())
            {
                var array = stream.StreamToByteArray();
                var result = array.ByteArrayToString();

                Assert.AreEqual(Data, result);
            }
        }

        [Test]
        public void RoundtripStringToStreamThenStreamToByteArrayThenByteArrayToStringWhereStreamToByteArrayCannonSeek()
        {
            using (var stream = Data.StringToStream())
            {
                var array = new NonSeekingStream(stream).StreamToByteArray();

                var result = array.ByteArrayToString();

                Assert.AreEqual(Data, result);
            }
        }

        [Test]
        public void RoundtripWithPrefixStringToStreamThenStreamToByteArrayThenByteArrayToString()
        {
            using (var stream = Data.StringToStream())
            {
                var array = stream.StreamToByteArray(new byte[] { 27, 34, 97 });

                Assert.AreEqual(27, array[0]);
                Assert.AreEqual(34, array[1]);
                Assert.AreEqual(97, array[2]);

                var tmp = new byte[array.Length - 3];
                for (var i = 0; i < tmp.Length; i++)
                    tmp[i] = array[i + 3];

                var result = tmp.ByteArrayToString();

                Assert.AreEqual(Data, result);
            }
        }

        [Test]
        public void RoundtripWithPrefixStringToStreamThenStreamToByteArrayThenByteArrayToStringWhereStreamToByteArrayCannonSeek()
        {
            using (var stream = Data.StringToStream())
            {
                var array = new NonSeekingStream(stream).StreamToByteArray(new byte[] { 27, 34, 97 });

                Assert.AreEqual(27, array[0]);
                Assert.AreEqual(34, array[1]);
                Assert.AreEqual(97, array[2]);

                var tmp = new byte[array.Length - 3];
                for (var i = 0; i < tmp.Length; i++)
                    tmp[i] = array[i + 3];

                var result = tmp.ByteArrayToString();

                Assert.AreEqual(Data, result);
            }
        }

        [Test]
        public void RoundtripStringToStreamThenStreamToString()
        {
            using (var stream = Data.StringToStream())
            {
                var result = stream.StreamToString();

                Assert.AreEqual(Data, result);
            }
        }

        [Test]
        public void RoundtripStringToStreamThenStreamToStringWhereBufferIsUnaccessible()
        {
            using (var stream = Data.StringToStream())
            {
                var memoryStreamWithInaccessibleBuffer = new MemoryStream(((MemoryStream) stream).ToArray(), 0,
                                                                          (int)stream.Length, false, false);

                var result = memoryStreamWithInaccessibleBuffer.StreamToString();

                Assert.AreEqual(Data, result);
            }
        }

        [Test]
        public void RoundtripStringToStreamThenCopyStreamThenStreamToString()
        {
            using (var stream = Data.StringToStream())
            {
                using (var stream2 = new MemoryStream())
                {
                    stream.CopyTo(stream2);

                    var result = stream2.ResetToBegining().StreamToString();

                    Assert.AreEqual(Data, result);
                }
            }
        }

        [Test]
        public void RoundtripStringToByteArrayThenByteArrayToString()
        {
            var array = Data.StringToByteArray();

            var result = array.ByteArrayToString();

            Assert.AreEqual(Data, result);
        }

        [Test]
        public void ResetToBeginingSeeksToStart()
        {
            using(var stream = new MemoryStream(new byte[50]))
            {
                stream.Seek(20, SeekOrigin.Begin);

                Assert.AreEqual(20, stream.Position);
                Assert.AreEqual(0, (int) stream.ResetToBegining().Position);
            }
        }

        [Test]
        public void ByteArrayToStreamWrapsFullArrayWithDefaultArguments()
        {
            var source = new byte[] { 7, 34, 56, 78, 92, 1 };

            using (var stream = source.ByteArrayToStream())
            {
                Assert.AreEqual(6, (int) stream.Length);

                var result = new byte[6];
                stream.Read(result, 0, 6);

                CollectionAssert.AreEqual(source, result);
            }
        }

        [Test]
        public void ByteArrayToStreamWrapsDefinedRegionByIndex()
        {
            var source = new byte[] { 7, 34, 56, 78, 92, 1 };

            using (var stream = source.ByteArrayToStream(1))
            {
                Assert.AreEqual(5, (int) stream.Length);

                var result = new byte[5];
                stream.Read(result, 0, 5);

                Assert.AreEqual(34, result[0]);
                Assert.AreEqual(56, result[1]);
                Assert.AreEqual(78, result[2]);
                Assert.AreEqual(92, result[3]);
                Assert.AreEqual(1, result[4]);
            }
        }

        [Test]
        public void ByteArrayToStreamWrapsDefinedRegionByIndexAndCount()
        {
            var source = new byte[] { 7, 34, 56, 78, 92, 1 };

            using (var stream = source.ByteArrayToStream(1, 4))
            {
                Assert.AreEqual(4, (int) stream.Length);

                var result = new byte[4];
                stream.Read(result, 0, 4);

                Assert.AreEqual(34, result[0]);
                Assert.AreEqual(56, result[1]);
                Assert.AreEqual(78, result[2]);
                Assert.AreEqual(92, result[3]);
            }
        }

        [Test]
        public void ResetToBeginingThrowsArgumentNulLExceptionForNullInputStream()
        {
            var exception = Assert.Catch<ArgumentNullException>(() => ((Stream)null).ResetToBegining());

            Assert.AreEqual("stream", exception.ParamName);
        }
    }
}
