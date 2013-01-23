using System.Collections.Generic;
using System.Web;
using DocaLabs.Http.Client.Serialization;
using DocaLabs.Http.Client.Serialization.ContentEncoding;
using DocaLabs.Http.Client.Tests.Serialization._Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Http.Client.Tests.Serialization
{
    [Subject(typeof(SerializeAsFormAttribute))]
    class when_serialize_as_form_attribute_is_used : request_serialization_test_context
    {
        static TestTarget original_object;
        static SerializeAsFormAttribute attribute;

        Establish context = () =>
        {
            original_object = new TestTarget
            {
                Value1 = 2012,
                Value2 = "Hello World!"
            };

            attribute = new SerializeAsFormAttribute();
        };

        Because of =
            () => attribute.Serialize(original_object, mock_web_request.Object);

        It should_set_request_content_type_as_url_encoded_form =
            () => mock_web_request.Object.ContentType.ShouldEqual("application/x-www-form-urlencoded");

        It should_serialize_all_properties =
            () => HttpUtility.ParseQueryString(GetRequestData()).ShouldContainOnly(
                new KeyValuePair<string, string>("Value1", "2012"),
                new KeyValuePair<string, string>("Value2", "Hello World!"));

        It should_properly_url_encode_values =
            () => GetRequestData().ShouldContain("Hello+World!");
    }

    [Subject(typeof(SerializeAsFormAttribute))]
    class when_serialize_as_form_attribute_is_used_with_gzip_content_encoding : request_serialization_test_context
    {
        static TestTarget original_object;
        static SerializeAsFormAttribute attribute;

        Establish context = () =>
        {
            original_object = new TestTarget
            {
                Value1 = 2012,
                Value2 = "Hello World!"
            };

            attribute = new SerializeAsFormAttribute { RequestContentEncoding = KnownContentEncodings.Gzip };
        };

        Because of =
            () => attribute.Serialize(original_object, mock_web_request.Object);

        It should_set_request_content_type_as_url_encoded_form =
            () => mock_web_request.Object.ContentType.ShouldEqual("application/x-www-form-urlencoded");

        It should_add_content_encoding_request_header =
            () => mock_web_request.Object.Headers.ShouldContain("content-encoding");

        It should_add_gzip_content_encoding =
            () => mock_web_request.Object.Headers["content-encoding"].ShouldEqual(KnownContentEncodings.Gzip);

        It should_serialize_all_properties =
            () => HttpUtility.ParseQueryString(GetDecodedRequestData()).ShouldContainOnly(
                new KeyValuePair<string, string>("Value1", "2012"),
                new KeyValuePair<string, string>("Value2", "Hello World!"));

        It should_properly_url_encode_values =
            () => GetDecodedRequestData().ShouldContain("Hello+World!");
    }
}
