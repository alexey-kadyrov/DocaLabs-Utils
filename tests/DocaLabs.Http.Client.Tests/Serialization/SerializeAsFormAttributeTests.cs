﻿using System.Collections.Generic;
using System.Text;
using System.Web;
using DocaLabs.Http.Client.Serialization;
using DocaLabs.Http.Client.Tests.Serialization._Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Http.Client.Tests.Serialization
{
    [Subject(typeof(SerializeAsFormAttribute))]
    class when_serialize_as_form_attribute_is_used : request_serialization_test_context
    {
        static SerializeAsFormAttribute attribute;

        Establish context = 
            () => attribute = new SerializeAsFormAttribute();

        Because of =
            () => attribute.Serialize(original_object, mock_web_request.Object);

        It should_set_request_content_type_as_url_encoded_form =
            () => mock_web_request.Object.ContentType.ShouldEqual("application/x-www-form-urlencoded");

        It should_serialize_all_properties =
            () => HttpUtility.ParseQueryString(Encoding.UTF8.GetString(request_data.ToArray())).ShouldContainOnly(
                new KeyValuePair<string, string>("Value1", "2012"),
                new KeyValuePair<string, string>("Value2", "Hello World!"));

        It should_properly_url_encode_values =
            () => Encoding.UTF8.GetString(request_data.ToArray()).ShouldContain("Hello+World!");
    }
}