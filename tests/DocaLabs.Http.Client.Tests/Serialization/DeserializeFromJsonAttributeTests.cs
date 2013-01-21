using System.IO;
using System.Text;
using DocaLabs.Http.Client.Serialization;
using DocaLabs.Http.Client.Tests.Serialization._Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Http.Client.Tests.Serialization
{
    [Subject(typeof(DeserializeFromJsonAttribute))]
    class when_deserialize_from_json_attribute_is_used : response_deserialization_test_context
    {
        const string data = "{Value1:2012, Value2:\"Hello World!\"}";
        static DeserializeFromJsonAttribute attribute;
        static TestTarget target;

        Establish context = () =>
        {
            attribute = new DeserializeFromJsonAttribute();
            Setup("application/json", new MemoryStream(Encoding.UTF8.GetBytes(data)));
        };

        Because of = 
            () => target = attribute.Deserialize<TestTarget>(http_response);

        It should_deserialize_object = () => target.ShouldBeSimilar(new TestTarget
        {
            Value1 = 2012,
            Value2 = "Hello World!"
        });
    }
}
