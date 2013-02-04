using System;
using System.IO;
using System.Text;
using DocaLabs.Http.Client.ResponseDeserialization;
using DocaLabs.Http.Client.Tests._Utils;
using Machine.Specifications;

namespace DocaLabs.Http.Client.Tests.ResponseDeserialization
{
    [Subject(typeof(PlainTextResponseDeserializer), "deserialization")]
    class when_plain_text_deserializer_is_used_for_string_result : response_deserialization_test_context
    {
        const string data = "Hello World!";
        static PlainTextResponseDeserializer deserializer;
        static string target;

        Establish context = () =>
        {
            deserializer = new PlainTextResponseDeserializer();
            Setup("text/plain", new MemoryStream(Encoding.UTF8.GetBytes(data)));
        };

        Because of =
            () => target = (string)deserializer.Deserialize(http_response, typeof(string));

        It should_deserialize_string = 
            () => target.ShouldEqual("Hello World!");
    }

    [Subject(typeof(PlainTextResponseDeserializer), "deserialization")]
    class when_plain_text_deserializer_is_used_for_decimal_result : response_deserialization_test_context
    {
        const string data = "42.55";
        static PlainTextResponseDeserializer deserializer;
        static decimal target;

        Establish context = () =>
        {
            deserializer = new PlainTextResponseDeserializer();
            Setup("text/plain", new MemoryStream(Encoding.UTF8.GetBytes(data)));
        };

        Because of =
            () => target = (decimal)deserializer.Deserialize(http_response, typeof(decimal));

        It should_deserialize_decimal =
            () => target.ShouldEqual(42.55M);
    }

    [Subject(typeof(PlainTextResponseDeserializer), "deserialization")]
    class when_plain_text_deserializer_is_used_with_empty_response_stream_for_string_result : response_deserialization_test_context
    {
        const string data = "";
        static PlainTextResponseDeserializer deserializer;
        static string target;

        Establish context = () =>
        {
            deserializer = new PlainTextResponseDeserializer();
            Setup("text/plain", new MemoryStream(Encoding.UTF8.GetBytes(data)));
        };

        Because of =
            () => target = (string)deserializer.Deserialize(http_response, typeof(string));

        It should_return_null =
            () => target.ShouldBeNull();
    }

    [Subject(typeof(PlainTextResponseDeserializer), "deserialization")]
    class when_plain_text_deserializer_is_used_with_empty_response_stream_for_decimal_result : response_deserialization_test_context
    {
        const string data = "";
        static PlainTextResponseDeserializer deserializer;
        static decimal target;

        Establish context = () =>
        {
            deserializer = new PlainTextResponseDeserializer();
            Setup("text/plain", new MemoryStream(Encoding.UTF8.GetBytes(data)));
        };

        Because of =
            () => target = (decimal)deserializer.Deserialize(http_response, typeof(decimal));

        It should_return_default_value_for_decimal =
            () => target.ShouldEqual(default(decimal));
    }

    [Subject(typeof(PlainTextResponseDeserializer), "deserialization")]
    class when_plain_text_deserializer_is_used_with_null_result_type : response_deserialization_test_context
    {
        const string data = "Hello World!";
        static Exception exception;
        static PlainTextResponseDeserializer deserializer;

        Establish context = () =>
        {
            deserializer = new PlainTextResponseDeserializer();
            Setup("text/plain", new MemoryStream(Encoding.UTF8.GetBytes(data)));
        };

        Because of =
            () => exception = Catch.Exception(() => deserializer.Deserialize(http_response, null));

        It should_throw_argument_null_exception =
            () => exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_result_type_argument =
            () => ((ArgumentNullException)exception).ParamName.ShouldEqual("resultType");
    }

    [Subject(typeof(PlainTextResponseDeserializer), "deserialization")]
    public class when_plain_text_deserializer_is_used_with_null_response : response_deserialization_test_context
    {
        static Exception exception;
        static PlainTextResponseDeserializer deserializer;

        Establish context =
            () => deserializer = new PlainTextResponseDeserializer();

        Because of =
            () => exception = Catch.Exception(() => deserializer.Deserialize(null, typeof(string)));

        It should_throw_argument_null_exception =
            () => exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_response_argument =
            () => ((ArgumentNullException)exception).ParamName.ShouldEqual("response");
    }
}
