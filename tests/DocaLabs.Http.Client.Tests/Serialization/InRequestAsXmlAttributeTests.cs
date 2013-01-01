using System.IO;
using System.Text;
using System.Xml.Linq;
using DocaLabs.Http.Client.Serialization;
using DocaLabs.Http.Client.Tests.Serialization._Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Http.Client.Tests.Serialization
{
    [Subject(typeof(InRequestAsXmlAttribute))]
    public class when_in_request_as_xml_attribute_is_used_for_serialization_in_default_configuration : RequestAttributeTestContext
    {
        static InRequestAsXmlAttribute attribute;

        Establish context = 
            () => attribute = new InRequestAsXmlAttribute();

        Because of = 
            () => attribute.Serialize(original_object, mock_web_request.Object);

        It should_set_request_content_type_as_xml =
            () => mock_web_request.Object.ContentType.ShouldBeEqualIgnoringCase("text/xml");

        It should_serialize_object =
            () => request_data.ToArray().ParseXml<TestTarget>().ShouldBeSimilar(original_object);

        It should_not_serialize_doctype_definition =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).DocumentType.ShouldBeNull();

        It should_use_tab_identation =
            () => Encoding.UTF8.GetString(request_data.ToArray()).ShouldContain("\t");

        It should_use_utf8_encoding =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).Declaration.Encoding.ShouldBeEqualIgnoringCase("utf-8");
    }

    [Subject(typeof(InRequestAsXmlAttribute))]
    public class when_in_request_as_xml_attribute_is_used_for_serialization_with_doc_type : RequestAttributeTestContext
    {
        // ReSharper disable PossibleNullReferenceException
        static InRequestAsXmlAttribute attribute;

        Establish context = () => attribute = new InRequestAsXmlAttribute
        {
            DocTypeName = "testService",
            Pubid = "-//Test//DTDTest testService v2//EN",
            Sysid = "http://dtd.foo.com/testService_v2.dtd"
        };

        Because of =
            () => attribute.Serialize(original_object, mock_web_request.Object);

        It should_set_request_content_type_as_xml =
            () => mock_web_request.Object.ContentType.ShouldBeEqualIgnoringCase("text/xml");

        It should_serialize_object =
            () => request_data.ToArray().ParseXml<TestTarget>().ShouldBeSimilar(original_object);

        It should_serialize_doctype_definition =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).DocumentType.ShouldNotBeNull();

        It should_serialize_specified_doctype_name =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).DocumentType.Name.ShouldEqual("testService");

        It should_serialize_specified_doctype_pubid =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).DocumentType.PublicId.ShouldEqual("-//Test//DTDTest testService v2//EN");

        It should_serialize_specified_doctype_sysid =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).DocumentType.SystemId.ShouldEqual("http://dtd.foo.com/testService_v2.dtd");

        It should_use_tab_identation =
            () => Encoding.UTF8.GetString(request_data.ToArray()).ShouldContain("\t");

        It should_use_utf8_encoding =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).Declaration.Encoding.ShouldBeEqualIgnoringCase("utf-8");
        // ReSharper restore PossibleNullReferenceException
    }

    [Subject(typeof(InRequestAsXmlAttribute))]
    public class when_in_request_as_xml_attribute_is_used_for_serialization_with_utf16_encoding : RequestAttributeTestContext
    {
        static InRequestAsXmlAttribute attribute;

        Establish context = () => attribute = new InRequestAsXmlAttribute
        {
            Encoding = CharEncoding.Utf16
        };

        Because of =
            () => attribute.Serialize(original_object, mock_web_request.Object);

        It should_set_request_content_type_as_xml =
            () => mock_web_request.Object.ContentType.ShouldBeEqualIgnoringCase("text/xml");

        It should_serialize_object =
            () => request_data.ToArray().ParseXml<TestTarget>().ShouldBeSimilar(original_object);

        It should_not_serialize_doctype_definition =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).DocumentType.ShouldBeNull();

        It should_use_tab_identation =
            () => Encoding.Unicode.GetString(request_data.ToArray()).ShouldContain("\t");

        It should_use_utf16_encoding =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).Declaration.Encoding.ShouldBeEqualIgnoringCase("utf-16");
    }

    [Subject(typeof(InRequestAsXmlAttribute))]
    public class when_in_request_as_xml_attribute_is_used_for_serialization_with_utf32_encoding : RequestAttributeTestContext
    {
        static InRequestAsXmlAttribute attribute;

        Establish context = () => attribute = new InRequestAsXmlAttribute
        {
            Encoding = CharEncoding.Utf32
        };

        Because of =
            () => attribute.Serialize(original_object, mock_web_request.Object);

        It should_set_request_content_type_as_xml =
            () => mock_web_request.Object.ContentType.ShouldBeEqualIgnoringCase("text/xml");

        It should_serialize_object =
            () => request_data.ToArray().ParseXml<TestTarget>().ShouldBeSimilar(original_object);

        It should_not_serialize_doctype_definition =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).DocumentType.ShouldBeNull();

        It should_use_tab_identation =
            () => Encoding.UTF32.GetString(request_data.ToArray()).ShouldContain("\t");

        It should_use_utf32_encoding =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).Declaration.Encoding.ShouldBeEqualIgnoringCase("utf-32");
    }

    [Subject(typeof(InRequestAsXmlAttribute))]
    public class when_in_request_as_xml_attribute_is_used_for_serialization_with_ascii_encoding : RequestAttributeTestContext
    {
        static InRequestAsXmlAttribute attribute;

        Establish context = () => attribute = new InRequestAsXmlAttribute
        {
            Encoding = CharEncoding.Ascii
        };

        Because of =
            () => attribute.Serialize(original_object, mock_web_request.Object);

        It should_set_request_content_type_as_xml =
            () => mock_web_request.Object.ContentType.ShouldBeEqualIgnoringCase("text/xml");

        It should_serialize_object =
            () => request_data.ToArray().ParseXml<TestTarget>().ShouldBeSimilar(original_object);

        It should_not_serialize_doctype_definition =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).DocumentType.ShouldBeNull();

        It should_use_tab_identation =
            () => Encoding.ASCII.GetString(request_data.ToArray()).ShouldContain("\t");

        It should_use_ascii_encoding =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).Declaration.Encoding.ShouldBeEqualIgnoringCase("us-ascii");
    }

    [Subject(typeof(InRequestAsXmlAttribute))]
    public class when_in_request_as_xml_attribute_is_used_for_serialization_without_identation : RequestAttributeTestContext
    {
        static InRequestAsXmlAttribute attribute;

        Establish context =
            () => attribute = new InRequestAsXmlAttribute { Indent = false };

        Because of =
            () => attribute.Serialize(original_object, mock_web_request.Object);

        It should_set_request_content_type_as_xml =
            () => mock_web_request.Object.ContentType.ShouldBeEqualIgnoringCase("text/xml");

        It should_serialize_object =
            () => request_data.ToArray().ParseXml<TestTarget>().ShouldBeSimilar(original_object);

        It should_not_serialize_doctype_definition =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).DocumentType.ShouldBeNull();

        It should_not_use_tab_identation =
            () => Encoding.UTF8.GetString(request_data.ToArray()).ShouldNotContain("\t");

        It should_use_utf8_encoding =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).Declaration.Encoding.ShouldBeEqualIgnoringCase("utf-8");
    }

    [Subject(typeof(InRequestAsXmlAttribute))]
    public class when_in_request_as_xml_attribute_is_used_for_serialization_with_redefined_ident_chars : RequestAttributeTestContext
    {
        static InRequestAsXmlAttribute attribute;

        Establish context =
            () => attribute = new InRequestAsXmlAttribute { IndentChars = "\r\r\r\r\r" };

        Because of =
            () => attribute.Serialize(original_object, mock_web_request.Object);

        It should_set_request_content_type_as_xml =
            () => mock_web_request.Object.ContentType.ShouldBeEqualIgnoringCase("text/xml");

        It should_serialize_object =
            () => request_data.ToArray().ParseXml<TestTarget>().ShouldBeSimilar(original_object);

        It should_not_serialize_doctype_definition =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).DocumentType.ShouldBeNull();

        It should_use_specified_chars_for_identation =
            () => Encoding.UTF8.GetString(request_data.ToArray()).ShouldContain("\r\r\r\r\r");

        It should_use_utf8_encoding =
            () => XDocument.Load(new MemoryStream(request_data.ToArray())).Declaration.Encoding.ShouldBeEqualIgnoringCase("utf-8");
    }

    [Subject(typeof(InRequestAsXmlAttribute))]
    public class when_in_request_as_xml_attribute_is_newed
    {
        static InRequestAsXmlAttribute attribute;

        Because of = 
            () => attribute = new InRequestAsXmlAttribute();

        It should_set_encoding_to_utf8 =
            () => attribute.Encoding.ShouldEqual(CharEncoding.Utf8);

        It should_set_ident_to_true =
            () => attribute.Indent.ShouldBeTrue();

        It should_set_ident_chars_to_tab =
            () => attribute.IndentChars.ShouldEqual("\t");

        It should_set_doc_type_name_to_null =
            () => attribute.DocTypeName.ShouldBeNull();

        It should_set_pubid_to_null =
            () => attribute.Pubid.ShouldBeNull();

        It should_set_sysid_to_null =
            () => attribute.Sysid.ShouldBeNull();

        It should_set_subset_to_null =
            () => attribute.Subset.ShouldBeNull();
    }
}
