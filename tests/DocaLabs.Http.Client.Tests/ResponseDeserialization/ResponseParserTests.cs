using System;
using System.Collections.Generic;
using System.IO;
using DocaLabs.Http.Client.ResponseDeserialization;
using DocaLabs.Http.Client.Tests._Utils;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Http.Client.Tests.ResponseDeserialization
{
    [Subject(typeof(ResponseParser))]
    class when_response_parser_is_used_in_deafult_configuration
    {
        It should_have_four_providers =
            () => ResponseParser.Providers.Count.ShouldEqual(4);

        It should_have_json_xml_plain_text_and_image_deserializers = () => ResponseParser.Providers.ShouldMatch(x =>
            x[0].GetType() == typeof(JsonResponseDeserializer) &&
            x[1].GetType() == typeof(XmlResponseDeserializer) &&
            x[2].GetType() == typeof(PlainTextResponseDeserializer) &&
            x[3].GetType() == typeof(ImageResponseDeserializer));
    }

    [Subject(typeof(ResponseParser))]
    class when_setting_a_new_collection_of_providers_and_parsing_for_types_that_can_be_deserialized : response_deserialization_test_context
    {
        static IList<IResponseDeserializationProvider> original_providers;

        Cleanup after_each =
            () => ResponseParser.Providers = original_providers;

        Establish context = () =>
        {
            Setup("application/json", new MemoryStream());

            original_providers = ResponseParser.Providers;

            ResponseParser.Providers = new List<IResponseDeserializationProvider>
            {
                new FirstDeserializer(),
                new SecondDeserializer(),
                new ThirdDeserializer()
            };
        };

        It should_have_three_providers =
            () => ResponseParser.Providers.Count.ShouldEqual(3);

        It should_have_configured_deserializers = () => ResponseParser.Providers.ShouldMatch(x =>
            x[0].GetType() == typeof(FirstDeserializer) &&
            x[1].GetType() == typeof(SecondDeserializer) &&
            x[2].GetType() == typeof(ThirdDeserializer));

        It should_be_able_to_deserialize_using_first_provider =
            () => ResponseParser.Parse(mock_request.Object, typeof(FirstResult)).ShouldBeOfType<FirstResult>();

        It should_be_able_to_deserialize_using_second_provider =
            () => ResponseParser.Parse(mock_request.Object, typeof(SecondResult)).ShouldBeOfType<SecondResult>();

        It should_be_able_to_deserialize_using_attribute_instead_of_third_provider =
            () => ResponseParser.Parse(mock_request.Object, typeof(ThirdResult)).ShouldBeOfType<ThirdResult>();

        It should_return_deafult_void_type_if_result_type_is_void_type =
            () => ResponseParser.Parse(mock_request.Object, typeof (VoidType)).ShouldNotBeTheSameAs(VoidType.Value);

        It should_throw_unrecoverable_http_client_exception_for_type_which_deserialization_is_unknown =
            () => Catch.Exception(() => ResponseParser.Parse(mock_request.Object, typeof(ForthResult))).ShouldBeOfType<UnrecoverableHttpClientException>();

        class FirstResult
        {
        }

        class SecondResult
        {
        }

        [AttributeForThirdDeserializer]
        class ThirdResult
        {
        }

        class ForthResult
        {
        }

        class FirstDeserializer : IResponseDeserializationProvider
        {
            public object Deserialize(HttpResponse response, Type resultType)
            {
                return new FirstResult();
            }

            public bool CanDeserialize(HttpResponse response, Type resultType)
            {
                return resultType == typeof (FirstResult);
            }
        }

        class SecondDeserializer : IResponseDeserializationProvider
        {
            public object Deserialize(HttpResponse response, Type resultType)
            {
                return new SecondResult();
            }

            public bool CanDeserialize(HttpResponse response, Type resultType)
            {
                return resultType == typeof(SecondResult);
            }
        }

        class ThirdDeserializer : IResponseDeserializationProvider
        {
            public object Deserialize(HttpResponse response, Type resultType)
            {
                return null;
            }

            public bool CanDeserialize(HttpResponse response, Type resultType)
            {
                return resultType == typeof(ThirdResult);
            }
        }

        class AttributeForThirdDeserializerAttribute : ResponseDeserializationAttribute
        {
            public override object Deserialize(HttpResponse response, Type resultType)
            {
                return new ThirdResult();
            }
        }
    }

    [Subject(typeof(ResponseParser))]
    class when_parsing_response_for_null_request
    {
        static Exception exception;

        Because of =
            () => exception = Catch.Exception(() => ResponseParser.Parse(null, typeof (TestTarget)));

        It should_throw_argument_null_exception =
            () => exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_request_parameter =
            () => ((ArgumentNullException) exception).ParamName.ShouldEqual("request");
    }

    [Subject(typeof(ResponseParser))]
    class when_parsing_response_for_null_result_type : response_deserialization_test_context
    {
        static Exception exception;

        Because of =
            () => exception = Catch.Exception(() => ResponseParser.Parse(mock_request.Object, null));

        It should_throw_argument_null_exception =
            () => exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_result_type_parameter =
            () => ((ArgumentNullException)exception).ParamName.ShouldEqual("resultType");
    }

    [Subject(typeof(ResponseParser))]
    class when_provider_collection_to_null
    {
        static Exception exception;

        Because of =
            () => exception = Catch.Exception(() => ResponseParser.Providers = null);

        It should_throw_argument_null_exception =
            () => exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_value_parameter =
            () => ((ArgumentNullException)exception).ParamName.ShouldEqual("value");
    }
}
