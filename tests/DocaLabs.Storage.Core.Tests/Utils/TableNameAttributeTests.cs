using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Tests.Utils
{
    [Subject(typeof(TableNameAttribute)), UnitTestTag]
    class when_table_name_attribute_is_newed
    {
        static TableNameAttribute attribute;

        Establish context =()=>
        {
            attribute = new TableNameAttribute("name_property_test");
        };

        It should_set_name_property_to_value_passed_in_constructor = () => 
            attribute.Name.ShouldEqual("name_property_test");
    }
}
