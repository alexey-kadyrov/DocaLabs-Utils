using System.IO;
using System.Net;
using Moq;

namespace DocaLabs.Http.Client.Tests.Serialization._Utils
{
    public class response_deserialization_test_context
    {
        public class TestTarget
        {
            public int Value1 { get; set; }
            public string Value2 { get; set; }
        }

        protected static Mock<WebResponse> mock_web_response;
        protected static HttpResponse http_response;

        protected static void Setup(string contentType, Stream stream)
        {
            mock_web_response = new Mock<WebResponse>();
            mock_web_response.SetupAllProperties();
            mock_web_response.Setup(x => x.GetResponseStream()).Returns(stream);
            mock_web_response.Object.ContentType = contentType;
            mock_web_response.Object.ContentLength = stream.Length;

            var request = new Mock<WebRequest>();
            request.Setup(x => x.GetResponse()).Returns(mock_web_response.Object);

            http_response = new HttpResponse(request.Object);
        }
    }
}
