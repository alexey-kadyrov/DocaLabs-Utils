using System;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Tests.Utils
{
    class SampleEntityConfiguredInFile
    {
    }

    [TableName("table-name-from-table-name-attribute")]
    class SampleEntityWithTableNameAttribute
    {
    }

    [System.Data.Linq.Mapping.Table(Name = "table-name-from-linq-mapping-table-attribute")]
    class SampleEntityWithLinqMappingTableAttribute
    {
         
    }
    
    class SamplePluralizedEntity
    {
    }

    class SampleEntityForResolvingHandler
    {
    }

    class SampleEntityForResolvedHandler
    {
    }

    class SampleEntityForCacheSpecification
    {
    }

    class EntityTableNameProviderTestsContext
    {
        protected static EntityTableNameProvider provider;
        protected static string table_name;

        Establish context = () => provider = new EntityTableNameProvider();
    }

    [Subject(typeof(EntityTableNameProvider)), UnitTestTag]
    class when_non_generic_resolve_overload_is_called_with_null_argument : EntityTableNameProviderTestsContext
    {
        static Exception actual_exception;

        Because of = () => actual_exception = Catch.Exception(() => provider.Resolve(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_entity_type_argument =
            () => ((ArgumentNullException) actual_exception).ParamName.ShouldEqual("entityType");
    }

    [Subject(typeof(EntityTableNameProvider)), UnitTestTag]
    class when_resolving_table_name_for_enity_described_in_configuration_file : EntityTableNameProviderTestsContext
    {
        Because of = () => table_name = provider.Resolve<SampleEntityConfiguredInFile>();

        It should_return_name_from_the_configuration_file =
            () => table_name.ShouldEqual("table-name-from-file");
    }

    [Subject(typeof(EntityTableNameProvider)), UnitTestTag]
    class when_resolving_table_name_for_enity_having_table_name_attribute : EntityTableNameProviderTestsContext
    {
        Because of = () => table_name = provider.Resolve<SampleEntityWithTableNameAttribute>();

        It should_return_name_defined_in_the_attribute =
            () => table_name.ShouldEqual("table-name-from-table-name-attribute");
    }

    [Subject(typeof(EntityTableNameProvider)), UnitTestTag]
    class when_resolving_table_name_for_enity_having_ling_mappimg_table_attribute : EntityTableNameProviderTestsContext
    {
        Because of = () => table_name = provider.Resolve<SampleEntityWithLinqMappingTableAttribute>();

        It should_return_name_defined_in_the_attribute =
            () => table_name.ShouldEqual("table-name-from-linq-mapping-table-attribute");
    }

    [Subject(typeof(EntityTableNameProvider)), UnitTestTag]
    class when_resolving_table_name_for_bare_enity : EntityTableNameProviderTestsContext
    {
        Because of = () => table_name = provider.Resolve<SamplePluralizedEntity>();

        It should_return_pluralized_name_based_on_entity_type_name =
            () => table_name.ShouldEqual("SamplePluralizedEntities");
    }

    [Subject(typeof(EntityTableNameProvider)), UnitTestTag]
    class when_resolving_table_name_for_enity_with_handled_resolving_entity_to_table_name_event : EntityTableNameProviderTestsContext
    {
        static void OnProviderResolvingEntityToTableName(object sender, EntityTableNameArgs e)
        {
            e.TableName = "resolving-entity-to-table-name";
        }

        Cleanup after_each =
            () => provider.ResolvingEntityToTableName -= OnProviderResolvingEntityToTableName;

        Because of = () =>
        {
            provider.ResolvingEntityToTableName += OnProviderResolvingEntityToTableName;
            table_name = provider.Resolve<SampleEntityForResolvingHandler>();
        };

        It should_return_name_set_in_the_event_handler =
            () => table_name.ShouldEqual("resolving-entity-to-table-name");
    }

    [Subject(typeof(EntityTableNameProvider)), UnitTestTag]
    class when_resolving_table_name_for_enity_with_handled_resolving_and_resolved_entity_to_table_name_events : EntityTableNameProviderTestsContext
    {
        static void OnProviderResolvingEntityToTableName(object sender, EntityTableNameArgs e)
        {
            e.TableName = "resolving-entity-to-table-name";
        }

        static void OnProviderResolvedEntityToTableName(object sender, EntityTableNameArgs e)
        {
            e.TableName = "resolved-entity-to-table-name";
        }

        Cleanup after_each = () =>
        {
            provider.ResolvingEntityToTableName -= OnProviderResolvingEntityToTableName;
            provider.ResolvedEntityToTableName -= OnProviderResolvedEntityToTableName;
        };

        Because of = () =>
        {
            provider.ResolvingEntityToTableName += OnProviderResolvingEntityToTableName;
            provider.ResolvedEntityToTableName += OnProviderResolvedEntityToTableName;

            table_name = provider.Resolve<SampleEntityForResolvedHandler>();
        };

        It should_return_name_set_in_the_resolved_event_handler =
            () => table_name.ShouldEqual("resolved-entity-to-table-name");
    }

    [Subject(typeof(EntityTableNameProvider)), UnitTestTag]
    class when_resolving_table_name_for_enity_with_handled_with_handled_event_only_after_first_time : EntityTableNameProviderTestsContext
    {
        static void OnProviderResolvingEntityToTableName(object sender, EntityTableNameArgs e)
        {
            e.TableName = "resolving-entity-to-table-name";
        }

        static void OnProviderResolvedEntityToTableName(object sender, EntityTableNameArgs e)
        {
            e.TableName = "resolved-entity-to-table-name";
        }

        Cleanup after_each = () =>
        {
            provider.ResolvingEntityToTableName -= OnProviderResolvingEntityToTableName;
            provider.ResolvedEntityToTableName -= OnProviderResolvedEntityToTableName;
        };

        Because of = () =>
        {
            // just to insure that the type definition is cached so the event handlers work anyway
            provider.Resolve<SampleEntityForCacheSpecification>();

            provider.ResolvingEntityToTableName += OnProviderResolvingEntityToTableName;
            provider.ResolvedEntityToTableName += OnProviderResolvedEntityToTableName;

            table_name = provider.Resolve<SampleEntityForCacheSpecification>();
        };

        It should_return_name_that_was_assigned_when_there_was_not_any_evant_handlers_set =
            () => table_name.ShouldEqual("SampleEntityForCacheSpecifications");
    }
}
