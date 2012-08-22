﻿using System.IO;
using System.Text;
using DocaLabs.Http.Client.Serialization;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Http.Client.Tests.Serialization
{
    [Subject(typeof(InResponseAsXmlAttribute))]
    class when_in_response_as_xml_attribute_is_used_to_deserialize : ResponseAttributeTestContext
    {
        const string data = "<TestTarget><Value1>2012</Value1><Value2>Hello World!</Value2></TestTarget>";
        static InResponseAsXmlAttribute attribute;
        static TestTarget target;

        Establish context = () =>
        {
            attribute = new InResponseAsXmlAttribute();
            Setup("text/xml", new MemoryStream(Encoding.UTF8.GetBytes(data)));
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
