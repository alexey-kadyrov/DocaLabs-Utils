using DocaLabs.Http.Client.Factory;
using DocaLabs.Http.Client.Integration.Tests.Setup;
using DocaLabs.Http.Client.Serialization;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;

namespace DocaLabs.Http.Client.Integration.Tests.Factory
{
    public class OutData
    {
        public int Value1 { get; set; }
        public string Value2 { get; set; }
    }

    public class InData1
    {
        public int Value1 { get; set; }
        public string Value2 { get; set; }
    }

    public class InData2
    {
        public int Value1 { get; set; }

        [InRequestAsJson]
        public string Value2 { get; set; }
    }

    public interface ITestService
    {
        [HttpService(ServiceUrl = "http://ipv4.fiddler:5701/TestService/GetAsJson")]
        OutData GetAsJson(InData1 query);

        [HttpService(ServiceUrl = "http://ipv4.fiddler:5701/TestService/PostAsJsonAsJson")]
        OutData PostAsJson(InData2 query);
        
        OutData GetAsXml(InData1 query);
        
        OutData PostAsXml(InData2 query);
    }

    [Subject(typeof(HttpClientFactory))]
    class when
    {
        static ITestService client;

        Establish context = () =>
        {
            TestServerSetup.Setup();
            client = HttpClientFactory.Create<ITestService>();
        };

        //It should_call_get_json = () => client.GetAsJson(new InData1 {Value1 = 42, Value2 = "Hello World!"}).ShouldBeSimilar(new OutData
        //{
        //    Value1 = 42,
        //    Value2 = "Hello World!"
        //});

        It should_call_get_json2 = () => client.PostAsJson(new InData2 { Value1 = 42, Value2 = "Hello World!" }).ShouldBeSimilar(new OutData
        {
            Value1 = 42,
            Value2 = "Hello World!"
        });
    }
}
