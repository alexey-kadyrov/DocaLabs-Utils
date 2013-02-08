using System.IO;
using System.IO.Compression;
using System.Text;
using DocaLabs.Http.Client.ContentEncoding;
using Machine.Specifications;

namespace DocaLabs.Http.Client.Tests.ContentEncoding
{
    [Subject(typeof(InflatingContentDecoder))]
    class when_inflating_decoder_is_used
    {
        static InflatingContentDecoder decoder;
        static MemoryStream comressed_stream;
        static Stream decompression_stream;

        Cleanup after_each =
            () => decompression_stream.Dispose();

        Establish context = () =>
        {
            decoder = new InflatingContentDecoder();

            using (var comressedStream = new MemoryStream())
            {
                using (var uncomressedStream = new MemoryStream(Encoding.UTF8.GetBytes("Hello World!")))
                using (var compressionStream = new DeflateStream(comressedStream, CompressionMode.Compress))
                {
                    uncomressedStream.CopyTo(compressionStream);
                }

                comressed_stream = new MemoryStream(comressedStream.ToArray());
            }
        };

        Because of =
            () => decompression_stream = decoder.GetDecompressionStream(comressed_stream);

        It should_return_gzip_stream =
            () => decompression_stream.ShouldBeOfType<DeflateStream>();

        It should_wrap_around_original_stream =
            () => ((DeflateStream)decompression_stream).BaseStream.ShouldBeTheSameAs(comressed_stream);

        It should_be_able_to_decomress =
            () => DecomressData().ShouldEqual("Hello World!");

        static string DecomressData()
        {
            var data = new MemoryStream();
            decompression_stream.CopyTo(data);
            decompression_stream.Dispose();
            return Encoding.UTF8.GetString(data.ToArray());
        }
    }
}
