using DocaLabs.Http.Client.Mapping;
using Machine.Specifications;

namespace DocaLabs.Http.Client.Tests.Mapping
{
    [Subject(typeof(ParsedType))]
    class when
    {
        Establish context = () =>
        {
            var s = typeof (string).GetType();
        };
    }
}
