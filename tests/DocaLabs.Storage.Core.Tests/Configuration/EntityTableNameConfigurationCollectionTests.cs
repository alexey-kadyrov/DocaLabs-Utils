using System;
using System.Configuration;
using DocaLabs.Storage.Core.Configuration;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It=Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Tests.Configuration
{
    [Subject(typeof(EntityTableNameCollection)), UnitTestTag]
    class when_entity_table_name_configuration_collection_is_newed
    {
        static EntityTableNameCollection collection;

        Establish context2 =
            () => collection = new EntityTableNameCollection();

        It can_be_changed_at_runtime = 
            () => collection.IsReadOnly().ShouldBeFalse();

        It should_be_empty = 
            () => collection.AllKeys.ShouldBeEmpty();
    }

    [Subject(typeof(EntityTableNameCollection)), UnitTestTag]
    class when_entity_table_name_configuration_collection_is_initialized_with_elements
    {
        static EntityTableNameCollection collection;

        Establish context = 
            () => collection = new EntityTableNameCollection();

        Because of = () =>
        {
            collection.Add(new EntityTableNameElement
            {
                Type = typeof (decimal),
                Table = "my-first-table"
            });
            collection.Add(new EntityTableNameElement
            {
                Type = typeof (short),
                Table = "my-second-table"
            });
        };

        It can_be_changed_at_runtime = 
            () => collection.IsReadOnly().ShouldBeFalse();
        
        It number_of_keys_should_match_number_added_elements = 
            () => collection.AllKeys.Length.ShouldEqual(2);

        It should_contain_keys_of_all_added_elements =
            () => collection.AllKeys.ShouldContain(typeof (decimal), typeof (short));

        It should_return_existing_element_with_expected_entity_type =
            () => collection[typeof(decimal)].Type.ShouldEqual(typeof(decimal));

        It should_return_existing_element_with_expected_table_name = 
            () => collection[typeof(decimal)].Table.ShouldEqual("my-first-table");

        It should_return_null_for_element_with_unknown_key = 
            () => collection[typeof (int)].ShouldBeNull();
    }

    [Subject(typeof(EntityTableNameCollection)), UnitTestTag]
    class when_adding_duplicate_key_to_entity_table_name_configuration_collection
    {
        static EntityTableNameCollection collection;
        static Exception actual_exception;

        Establish context = 
            () => collection = new EntityTableNameCollection();

        Because of = () =>
        {
            collection.Add(new EntityTableNameElement
            {
                Type = typeof (decimal),
                Table = "my-first-table"
            });
            actual_exception = Catch.Exception(() => collection.Add(new EntityTableNameElement
            {
                Type = typeof (decimal),
                Table = "my-second-table"
            }));
        };

        It configuration_errors_exception_should_be_thrown = 
            () => actual_exception.ShouldBeOfType<ConfigurationErrorsException>();
    }

    [Subject(typeof(EntityTableNameCollection)), UnitTestTag]
    class when_removing_existing_element_from_entity_table_name_configuration_collection
    {
        static EntityTableNameCollection collection;

        Establish context = () => collection = new EntityTableNameCollection
        {
            new EntityTableNameElement
            {
                Type = typeof(decimal),
                Table = "my-first-table"
            },
            new EntityTableNameElement
            {
                Type = typeof(short),
                Table = "my-second-table"
            }
        };

        Because of = 
            () => collection.Remove(typeof (decimal));

        It should_contain_number_of_keys_minus_one = 
            () => collection.AllKeys.Length.ShouldEqual(1);

        It should_contain_keys_of_not_removed_elements =
            () => collection.AllKeys.ShouldContain(typeof(short));

        It should_return_existing_element_with_expected_entity_type =
            () => collection[typeof(short)].Type.ShouldEqual(typeof(short));

        It should_return_existing_element_with_expected_table_name =
            () => collection[typeof(short)].Table.ShouldEqual("my-second-table");

        It should_return_null_for_removed_element =
            () => collection[typeof(decimal)].ShouldBeNull();

    }

    [Subject(typeof(EntityTableNameCollection)), UnitTestTag]
    class when_removing_non_existing_element_from_entity_table_name_configuration_collection
    {
        static EntityTableNameCollection collection;

        Establish context = () => collection = new EntityTableNameCollection
        {
            new EntityTableNameElement
            {
                Type = typeof (decimal),
                Table = "my-first-table"
            },
            new EntityTableNameElement
            {
                Type = typeof (short),
                Table = "my-second-table"
            }
        };

        Because of =
            () => collection.Remove(typeof(int));

        It number_of_keys_should_still_match_number_added_elements =
            () => collection.AllKeys.Length.ShouldEqual(2);

        It should_still_contain_keys_of_all_added_elements =
            () => collection.AllKeys.ShouldContain(typeof(decimal), typeof(short));

        It should_return_existing_element_with_expected_entity_type =
            () => collection[typeof(decimal)].Type.ShouldEqual(typeof(decimal));

        It should_return_existing_element_with_expected_table_name =
            () => collection[typeof(decimal)].Table.ShouldEqual("my-first-table");
    }

    [Subject(typeof(EntityTableNameCollection)), UnitTestTag]
    class when_clearing_entity_table_name_configuration_collection
    {
        static EntityTableNameCollection collection;

        Establish context = () => collection = new EntityTableNameCollection
        {
            new EntityTableNameElement
            {
                Type = typeof(decimal),
                Table = "my-first-table"
            },
            new EntityTableNameElement
            {
                Type = typeof(short),
                Table = "my-second-table"
            }
        };

        Because of =
            () => collection.Clear();

        It should_not_be_any_keys_in_collection =
            () => collection.AllKeys.ShouldBeEmpty();

        It should_return_null_for_previously_existed_element =
            () => collection[typeof(decimal)].ShouldBeNull();
    }
}
