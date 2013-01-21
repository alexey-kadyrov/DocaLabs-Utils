using System.IO;
using System.Net;
using Machine.Specifications;
using Moq;

namespace DocaLabs.Http.Client.Tests.Serialization._Utils
{
    public class request_serialization_test_context
    {
        public class TestTarget
        {
            public int Value1 { get; set; }
            public string Value2 { get; set; }
        }

        protected static TestTarget original_object;
        protected static MemoryStream request_data;
        protected static Mock<WebRequest> mock_web_request;

        Establish context = () =>
        {
            request_data = new MemoryStream();

            mock_web_request = new Mock<WebRequest>();
            mock_web_request.SetupProperty(x => x.ContentType);
            mock_web_request.Setup(x => x.GetRequestStream()).Returns(request_data);

            original_object = new TestTarget
            {
                Value1 = 2012,
                Value2 = "Hello World!"
            };
        };

        Cleanup after_each =
            () => request_data.Dispose();
    }
}
