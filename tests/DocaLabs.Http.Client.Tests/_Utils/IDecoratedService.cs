using DocaLabs.Http.Client.RequestSerialization;

namespace DocaLabs.Http.Client.Tests._Utils
{
    [InterfaceOnly, ClassAttributeWithFields(Value = "Hello World!"), SerializeAsJson(CharSet = "something")]
    public interface IDecoratedService
    {
        TestResult GetResult(TestsQuery query);
    }
}