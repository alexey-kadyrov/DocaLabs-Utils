using DocaLabs.Http.Client.Factory;
using Machine.Specifications;

namespace DocaLabs.Http.Client.Tests.Factory
{
    public class MyQuery1
    {
    }

    public class MyResult1
    {
    }

    public class MyQuery2
    {
    }

    public class MyResult2
    {
    }

    public interface IMyServices
    {
        MyResult1 GetSmallData(MyQuery1 query);
        MyResult2 GetBigData(MyQuery2 query);
    }

    [Subject(typeof(HttpClientFactory))]
    class when
    {
        static IMyServices client;

        Because of =
            () => client = HttpClientFactory.Create<IMyServices>();

        It should_be_created =
            () => client.ShouldNotBeNull();
    }
}
