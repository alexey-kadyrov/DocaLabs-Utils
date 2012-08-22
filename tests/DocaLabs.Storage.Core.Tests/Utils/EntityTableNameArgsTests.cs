using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Tests.Utils
{
    [Subject(typeof(TableNameAttribute)), UnitTestTag]
    class when_entitytablenameargs_is_newed
    {
        static EntityTableNameArgs args;

        Establish context = () =>
        {
            args = new EntityTableNameArgs(typeof(int));
        };

        It should_set_type_property_to_value_passed_in_constructor = () =>
            args.Type.ShouldEqual(typeof(int));
    }
}
