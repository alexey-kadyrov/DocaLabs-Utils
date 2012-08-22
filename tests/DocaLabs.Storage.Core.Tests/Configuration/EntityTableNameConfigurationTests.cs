using System;
using DocaLabs.Storage.Core.Configuration;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Tests.Configuration
{
    class FromConfigration
    {
    }

    [Subject(typeof(EntityTableNameSection)), UnitTestTag]
    class when_entity_table_name_configuration_section_is_newed
    {
        static EntityTableNameSection section;

        Establish context = 
            () => section = new EntityTableNameSection();

        It can_be_changed_at_runtime = 
            () => section.IsReadOnly().ShouldBeFalse();

        It map_collection_should_be_empty = 
            () => section.Map.AllKeys.ShouldBeEmpty();
    }

    [Subject(typeof(EntityTableNameSection)), UnitTestTag]
    class when_entity_table_name_configuration_section_is_loaded_from_configuration_file
    {
        static EntityTableNameSection section;

        Establish context = 
            () => section = EntityTableNameSection.GetDefaultSection();

        It can_be_changed_at_runtime = 
            () => section.IsReadOnly().ShouldBeFalse();

        It map_should_contain_element_with_configured_entity_type = 
            () => section.Map[typeof(FromConfigration)].Type.ShouldEqual(typeof(FromConfigration));

        It map_should_contain_element_with_configured_table_name = 
            () => section.Map[typeof (FromConfigration)].Table.ShouldEqual("table-for-FromConfigration");
    }

    [Subject(typeof(EntityTableNameSection)), UnitTestTag]
    class when_get_section_is_called_with_null_argument
    {
        static Exception actual_exception;

        Establish context =
            () => actual_exception = Catch.Exception(() => EntityTableNameSection.GetSection(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_entity_type_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("sectionName");
    }
}
