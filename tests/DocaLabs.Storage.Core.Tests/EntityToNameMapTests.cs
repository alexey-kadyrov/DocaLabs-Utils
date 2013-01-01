using System;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Tests
{
    [System.ComponentModel.DataAnnotations.Schema.Table("table-name-that-should-be-not-used-for-resolving")]
    class EntityConfiguredExplicitly1
    {
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("table-name-that-should-be-not-used-for-resolving")]
    class EntityConfiguredExplicitly2
    {
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("table-name-that-should-be-not-used-for-resolving")]
    class EntityConfiguredExplicitly3
    {
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("table-name-that-should-be-not-used-for-resolving")]
    class EntityConfiguredExplicitly4
    {
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("table-name-that-should-be-not-used-for-resolving")]
    class EntityConfiguredExplicitly5
    {
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("table-name-that-should-be-not-used-for-resolving")]
    class EntityConfiguredExplicitly6
    {
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("table-name-from-table-attribute-for-generic-get")]
    class EntityWithTableAttribute1
    {
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("table-name-from-table-attribute-for-non-generic-get")]
    class EntityWithTableAttribute2
    {
    }
    
    class Pluralized1Entity
    {
    }

    class Pluralized2Entity
    {
    }

    [Subject(typeof(EntityToNameMap))]
    class when_non_generic_get_overload_is_called_with_null_argument
    {
        static Exception actual_exception;

        Because of = () => actual_exception = Catch.Exception(() => EntityToNameMap.Get(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_entity_type_argument =
            () => ((ArgumentNullException) actual_exception).ParamName.ShouldEqual("entityType");
    }

    [Subject(typeof(EntityToNameMap))]
    class when_resolving_name_for_enity_configured_explicitly_using_generic_get
    {
        static string table_name;

        Establish context =
            () => EntityToNameMap.Configure<EntityConfiguredExplicitly1>("explicitly-configured-table-name-for-generic-get");

        Because of = 
            () => table_name = EntityToNameMap.Get<EntityConfiguredExplicitly1>();

        It should_return_configured_name =
            () => table_name.ShouldEqual("explicitly-configured-table-name-for-generic-get");
    }

    [Subject(typeof(EntityToNameMap))]
    class when_resolving_name_for_enity_configured_explicitly_using_non_generic_get
    {
        static string table_name;

        Establish context =
            () => EntityToNameMap.Configure<EntityConfiguredExplicitly2>("explicitly-configured-table-name-for-non-generic-get");

        Because of =
            () => table_name = EntityToNameMap.Get(typeof(EntityConfiguredExplicitly2));

        It should_return_configured_name =
            () => table_name.ShouldEqual("explicitly-configured-table-name-for-non-generic-get");
    }

    [Subject(typeof(EntityToNameMap))]
    class when_resolving_name_for_enity_configured_explicitly_using_non_generic_configure
    {
        static string table_name;

        Establish context =
            () => EntityToNameMap.Configure(typeof(EntityConfiguredExplicitly3), "explicitly-configured-table-name-for-non-generic-configure");

        Because of =
            () => table_name = EntityToNameMap.Get<EntityConfiguredExplicitly3>();

        It should_return_configured_name =
            () => table_name.ShouldEqual("explicitly-configured-table-name-for-non-generic-configure");
    }

    [Subject(typeof(EntityToNameMap))]
    class when_resolving_name_for_enity_configured_explicitly_using_non_generic_configure_and_get
    {
        static string table_name;

        Establish context =
            () => EntityToNameMap.Configure(typeof(EntityConfiguredExplicitly4), "explicitly-configured-table-name-for-non-generic-configure-and-get");

        Because of =
            () => table_name = EntityToNameMap.Get(typeof(EntityConfiguredExplicitly4));

        It should_return_configured_name =
            () => table_name.ShouldEqual("explicitly-configured-table-name-for-non-generic-configure-and-get");
    }

    [Subject(typeof(EntityToNameMap))]
    class when_resolving_name_for_enity_that_was_configured_twice_using_generic_configure
    {
        static string table_name;

        Establish context = () =>
        {
            EntityToNameMap.Configure(typeof(EntityConfiguredExplicitly5), "explicitly-configured-table-name-for-generic-configure-1");
            EntityToNameMap.Configure<EntityConfiguredExplicitly5>("explicitly-configured-table-name-for-generic-configure-2");
        };

        Because of =
            () => table_name = EntityToNameMap.Get<EntityConfiguredExplicitly5>();

        It should_return_configured_name =
            () => table_name.ShouldEqual("explicitly-configured-table-name-for-generic-configure-2");
    }

    [Subject(typeof(EntityToNameMap))]
    class when_resolving_name_for_enity_that_was_configured_twice_using_non_generic_configure
    {
        static string table_name;

        Establish context = () =>
        {
            EntityToNameMap.Configure<EntityConfiguredExplicitly6>("explicitly-configured-table-name-for-non-generic-configure-1");
            EntityToNameMap.Configure(typeof(EntityConfiguredExplicitly6), "explicitly-configured-table-name-for-non-generic-configure-2");
        };

        Because of =
            () => table_name = EntityToNameMap.Get<EntityConfiguredExplicitly6>();

        It should_return_configured_name =
            () => table_name.ShouldEqual("explicitly-configured-table-name-for-non-generic-configure-2");
    }

    [Subject(typeof(EntityToNameMap))]
    class when_resolving_name_for_enity_with_table_attribute_using_generic_get
    {
        static string table_name;

        Because of =
            () => table_name = EntityToNameMap.Get<EntityWithTableAttribute1>();

        It should_return_name_from_the_table_attribute =
            () => table_name.ShouldEqual("table-name-from-table-attribute-for-generic-get");
    }

    [Subject(typeof(EntityToNameMap))]
    class when_resolving_name_for_enity_with_table_attribute_using_non_generic_get
    {
        static string table_name;

        Because of =
            () => table_name = EntityToNameMap.Get<EntityWithTableAttribute2>();

        It should_return_name_from_the_table_attribute =
            () => table_name.ShouldEqual("table-name-from-table-attribute-for-non-generic-get");
    }

    [Subject(typeof(EntityToNameMap))]
    class when_resolving_name_for_enity_using_generic_get
    {
        static string table_name;

        Because of =
            () => table_name = EntityToNameMap.Get<Pluralized1Entity>();

        It should_return_pluralized_name_derived_from_the_entity_type =
            () => table_name.ShouldEqual("Pluralized1Entities");
    }

    [Subject(typeof(EntityToNameMap))]
    class when_resolving_name_for_enity_using_non_generic_get
    {
        static string table_name;

        Because of =
            () => table_name = EntityToNameMap.Get<Pluralized2Entity>();

        It should_return_pluralized_name_derived_from_the_entity_type =
            () => table_name.ShouldEqual("Pluralized2Entities");
    }
}
