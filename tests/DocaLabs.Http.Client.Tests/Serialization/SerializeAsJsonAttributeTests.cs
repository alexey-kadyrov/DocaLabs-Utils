using DocaLabs.Http.Client.Serialization;
using DocaLabs.Http.Client.Serialization.ContentEncoding;
using DocaLabs.Http.Client.Tests.Serialization._Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Http.Client.Tests.Serialization
{
    [Subject(typeof(SerializeAsJsonAttribute))]
    class when_serialize_as_json_attribute_is_used : request_serialization_test_context
    {
        static TestTarget original_object;
        static SerializeAsJsonAttribute attribute;

        Establish context = () =>
        {
            original_object = new TestTarget
            {
                Value1 = 2012,
                Value2 = "Hello World!"
            };

            attribute = new SerializeAsJsonAttribute();
        };

        Because of =
            () => attribute.Serialize(original_object, mock_web_request.Object);

        It should_set_request_content_type_as_application_slash_json =
            () => mock_web_request.Object.ContentType.ShouldBeEqualIgnoringCase("application/json");

        It should_serialize_object =
            () => ParseRequestDataAsJson<TestTarget>().ShouldBeSimilar(original_object);
    }

    [Subject(typeof(SerializeAsJsonAttribute))]
    class when_serialize_as_json_attribute_is_used_with_gzip_content_encoding : request_serialization_test_context
    {
        static TestTarget original_object;
        static SerializeAsJsonAttribute attribute;

        Establish context = () =>
        {
            original_object = new TestTarget
            {
                Value1 = 2012,
                Value2 = "Hello World!"
            };

            attribute = new SerializeAsJsonAttribute { RequestContentEncoding = KnownContentEncodings.Gzip };
        };

        Because of =
            () => attribute.Serialize(original_object, mock_web_request.Object);

        It should_set_request_content_type_as_application_slash_json =
            () => mock_web_request.Object.ContentType.ShouldBeEqualIgnoringCase("application/json");

        It should_add_content_encoding_request_header =
            () => mock_web_request.Object.Headers.ShouldContain("content-encoding");

        It should_add_gzip_content_encoding =
            () => mock_web_request.Object.Headers["content-encoding"].ShouldEqual(KnownContentEncodings.Gzip);

        It should_serialize_object =
            () => ParseDecodedRequestDataAsJson<TestTarget>().ShouldBeSimilar(original_object);
    }
}
