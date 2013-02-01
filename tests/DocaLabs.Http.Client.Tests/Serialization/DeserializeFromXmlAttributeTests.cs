using System.IO;
using System.Text;
using DocaLabs.Http.Client.Deserialization;
using DocaLabs.Http.Client.Tests.Serialization._Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Http.Client.Tests.Serialization
{
    [Subject(typeof(DeserializeFromXmlAttribute))]
    class when_deserialize_from_xml_attribute_is_used : response_deserialization_test_context
    {
        const string data = "<TestTarget><Value1>2012</Value1><Value2>Hello World!</Value2></TestTarget>";
        static DeserializeFromXmlAttribute attribute;
        static TestTarget target;

        Establish context = () =>
        {
            attribute = new DeserializeFromXmlAttribute();
            Setup("text/xml", new MemoryStream(Encoding.UTF8.GetBytes(data)));
        };

        Because of =
            () => target = (TestTarget)attribute.Deserialize(http_response, typeof(TestTarget));

        It should_deserialize_object = () => target.ShouldBeSimilar(new TestTarget
        {
            Value1 = 2012,
            Value2 = "Hello World!"
        });
    }
}
