using System.IO;
using System.Net;
using Machine.Specifications;
using Machine.Specifications.Annotations;
using Moq;

namespace DocaLabs.Http.Client.Tests.Serialization
{
    // ReSharper disable InconsistentNaming
    public class RequestAttributeTestContext
    {
        public class TestTarget
        {
            public int Value1 { get; set; }
            public string Value2 { get; set; }
        }

        protected static TestTarget original_object;
        protected static MemoryStream request_data;
        protected static Mock<WebRequest> mock_web_request;

        [UsedImplicitly] Establish context = () =>
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

        [UsedImplicitly] Cleanup after_each =
            () => request_data.Dispose();
    }
    // ReSharper restore InconsistentNaming
}
