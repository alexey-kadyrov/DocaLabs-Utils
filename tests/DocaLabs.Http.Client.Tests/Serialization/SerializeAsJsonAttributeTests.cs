using DocaLabs.Http.Client.Serialization;
using DocaLabs.Http.Client.Tests.Serialization._Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Http.Client.Tests.Serialization
{
    [Subject(typeof(SerializeAsJsonAttribute))]
    class when_serialize_as_json_attribute_is_used : request_serialization_test_context
    {
        static SerializeAsJsonAttribute attribute;

        Establish context = 
            () => attribute = new SerializeAsJsonAttribute();

        Because of =
            () => attribute.Serialize(original_object, mock_web_request.Object);

        It should_set_request_content_type_as_application_slash_json =
            () => mock_web_request.Object.ContentType.ShouldBeEqualIgnoringCase("application/json");

        It should_serialize_object =
            () => request_data.ToArray().ParseJson<TestTarget>().ShouldBeSimilar(original_object);
    }
}
