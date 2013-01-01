using System;
using System.IO;
using System.Net;
using System.Text;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace DocaLabs.Http.Client.Tests
{
    public class HttpResponseInBodyTestClass
    {
        public int Value1 { get; set; }
        public string Value2 { get; set; }
    }

    class HttpResponseTestContext
    {
        public Mock<WebResponse> MockWebResponse { get; private set; }

        public HttpResponseTestContext(string contentType, Stream stream)
        {
            MockWebResponse = new Mock<WebResponse>();
            MockWebResponse.SetupAllProperties();
            MockWebResponse.Setup(x => x.GetResponseStream()).Returns(stream);
            MockWebResponse.Object.ContentType = contentType;
            
            if (stream != null) 
                MockWebResponse.Object.ContentLength = stream.Length;
        }
    }

    [Subject(typeof(HttpResponse))]
    class when_http_response_is_disposed_it_releases_all_resources
    {
        static HttpResponseTestContext test_context;
        static HttpResponse response;
        static Stream response_stream;

        Establish context = () =>
        {
            response_stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello World!"));
            test_context = new HttpResponseTestContext("text/plain", response_stream);
            response = new HttpResponse(test_context.MockWebResponse.Object);
        };

        Because of =
            () => response.Dispose();

        It should_close_the_underlying_web_response =
            () => test_context.MockWebResponse.Verify(x => x.Close(), Times.AtLeastOnce());

        It should_dispose_the_response_stream =
            () => (Catch.Exception(() => response_stream.ReadByte()) as ObjectDisposedException).ShouldNotBeNull();
    }

    [Subject(typeof(HttpResponse))]
    class when_http_response_is_newed_with_null_web_request
    {
        static Exception exception;

        Because of =
            () => exception = Catch.Exception(() => new HttpResponse(null));

        It should_throw_argument_null_exception =
            () => exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_request_argument =
            () => ((ArgumentNullException)exception).ParamName.ShouldEqual("response");
    }

    [Subject(typeof(HttpResponse))]
    class when_http_response_is_newed_and_the_web_request_returns_null_response
    {
        static Exception exception;

        Because of =
            () => exception = Catch.Exception(() => new HttpResponse(null));

        It should_throw_http_client_exception =
            () => exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_that_the_response_is_null =
            () => ((ArgumentNullException)exception).ParamName.ShouldContain("response");
    }

    [Subject(typeof(HttpResponse))]
    class when_http_response_is_newed_and_the_stream_is_null
    {
        static HttpResponseTestContext test_context;
        static Exception exception;

        Establish context =
            () => test_context = new HttpResponseTestContext("text/plain", null);

        Because of =
            () => exception = Catch.Exception(() => new HttpResponse(test_context.MockWebResponse.Object));

        It should_throw_http_client_exception =
            () => exception.ShouldBeOfType<HttpClientException>();

        It should_report_that_the_response_stream_is_null =
            () => exception.Message.ShouldContain("Response stream is null");
    }

    [Subject(typeof(HttpResponse))]
    class when_http_response_is_used_with_byte_array_data
    {
        static HttpResponseTestContext test_context;
        static HttpResponse response;
        static Stream response_stream;

        Establish context = () =>
        {
            response_stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello World!"));
            test_context = new HttpResponseTestContext("application/octet-stream", response_stream);
        };

        Because of =
            () => response = new HttpResponse(test_context.MockWebResponse.Object);

        It should_not_report_that_it_contains_object_serialized_as_xml =
            () => response.IsXml().ShouldBeFalse();

        It should_not_report_that_it_contains_object_serialized_as_json =
            () => response.IsJson().ShouldBeFalse();

        It should_return_all_byte_array_data =
            () => Encoding.UTF8.GetString(response.AsByteArray()).ShouldEqual("Hello World!");
    }

    [Subject(typeof(HttpResponse))]
    class when_http_response_is_used_with_plain_text_data
    {
        static HttpResponseTestContext test_context;
        static HttpResponse response;
        static Stream response_stream;

        Establish context = () =>
        {
            response_stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello World!"));
            test_context = new HttpResponseTestContext("text/plain", response_stream);
        };

        Because of =
            () => response = new HttpResponse(test_context.MockWebResponse.Object);

        It should_not_report_that_it_contains_object_serialized_as_xml =
            () => response.IsXml().ShouldBeFalse();

        It should_not_report_that_it_contains_object_serialized_as_json =
            () => response.IsJson().ShouldBeFalse();

        It should_deserialize_string_data =
            () => response.AsString().ShouldEqual("Hello World!");
    }

    [Subject(typeof(HttpResponse))]
    class when_http_response_is_used_with_object_encoded_as_json
    {
        static HttpResponseTestContext test_context;
        static HttpResponse response;
        static Stream response_stream;

        Establish context = () =>
        {
            response_stream = new MemoryStream(Encoding.UTF8.GetBytes("{Value1:2012, Value2: \"Hello World!\"}"));
            test_context = new HttpResponseTestContext("application/json", response_stream);
        };

        Because of =
            () => response = new HttpResponse(test_context.MockWebResponse.Object);

        It should_not_report_that_it_contains_object_serialized_as_xml =
            () => response.IsXml().ShouldBeFalse();

        It should_report_that_it_contains_object_serialized_as_json =
            () => response.IsJson().ShouldBeTrue();

        It should_deserialize_json_object = () => response.AsJsonObject<HttpResponseInBodyTestClass>().ShouldBeSimilar(new HttpResponseInBodyTestClass
        {
            Value1 = 2012,
            Value2 = "Hello World!"
        });
    }

    [Subject(typeof(HttpResponse))]
    class when_http_response_is_used_with_object_encoded_as_xml
    {
        static HttpResponseTestContext test_context;
        static HttpResponse response;
        static Stream response_stream;

        Establish context = () =>
        {
            response_stream = new MemoryStream(Encoding.UTF8.GetBytes("<HttpResponseInBodyTestClass><Value1>2012</Value1><Value2>Hello World!</Value2></HttpResponseInBodyTestClass>"));
            test_context = new HttpResponseTestContext("text/xml", response_stream);
        };

        Because of =
            () => response = new HttpResponse(test_context.MockWebResponse.Object);

        It should_report_that_it_contains_object_serialized_as_xml =
            () => response.IsXml().ShouldBeTrue();

        It should_not_report_that_it_contains_object_serialized_as_json =
            () => response.IsJson().ShouldBeFalse();

        It should_deserialize_json_object = () => response.AsXmlObject<HttpResponseInBodyTestClass>().ShouldBeSimilar(new HttpResponseInBodyTestClass
        {
            Value1 = 2012,
            Value2 = "Hello World!"
        });
    }
}
