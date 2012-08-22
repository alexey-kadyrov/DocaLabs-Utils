using DocaLabs.Storage.Core.Configuration;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It=Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Tests.Configuration
{
    [Subject(typeof(EntityTableNameElement)), UnitTestTag]
    class when_entity_table_name_configuration_element_is_newed
    {
        static EntityTableNameElement element;

        Establish context = 
            () => element = new EntityTableNameElement();

        It can_be_changed_at_runtime = 
            () => element.IsReadOnly().ShouldBeFalse();

        It entity_property_should_be_null = 
            () => element.Type.ShouldBeNull();

        It table_property_should_be_empty = 
            () => element.Table.ShouldBeEmpty();
    }

    [Subject(typeof(EntityTableNameElement)), UnitTestTag]
    class when_entity_property_is_set_on_entity_table_name_configuration_element
    {
        static EntityTableNameElement element;

        Establish context = 
            () => element = new EntityTableNameElement();

        Because of = 
            () => element.Type = typeof(int);

        It should_return_new_type =
            () => element.Type.ShouldEqual(typeof(int));
    }

    [Subject(typeof(EntityTableNameElement)), UnitTestTag]
    class when_table_property_is_set_on_entity_table_name_configuration_element
    {
        static EntityTableNameElement element;

        Establish context = 
            () => element = new EntityTableNameElement();

        Because of = 
            () => element.Table = "my-new-table";

        It should_return_new_table_name = 
            () => element.Table.ShouldEqual("my-new-table");
    }
}
