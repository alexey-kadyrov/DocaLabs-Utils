namespace DocaLabs.Http.Client.Tests._Utils
{
    public interface IServiceWithQueryAndResult
    {
        TestResult GetResult(TestsQuery query);
    }
}