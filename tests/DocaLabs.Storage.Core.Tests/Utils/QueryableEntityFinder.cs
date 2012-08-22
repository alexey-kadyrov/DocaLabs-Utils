using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.MSpec;
using DocaLabs.Utils.Conversion;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Tests.Utils
{
    [DataServiceKey("PartitionKey", "RowKey")]
    class SampleEntity1
    {
        public Guid PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Value { get; set; }
    }

    class SampleEntity2
    {
        [System.ComponentModel.DataAnnotations.Key]
        public Guid Key { get; set; }

        public string Value { get; set; }
    }

    class SampleEntity3
    {
        public Guid SampleEntity3Id { get; set; }

        public string Value { get; set; }
    }

    class SampleEntity4
    {
        [System.ComponentModel.DataAnnotations.Key]
        public Guid PartitionKey { get; set; }

        [System.ComponentModel.DataAnnotations.Key]
        public string RowKey { get; set; }

        public string Value { get; set; }
    }

    class SampleEntity5
    {
        public Guid Key { get; set; }

        public string Value { get; set; }
    }

    [DataServiceKey("UnknownPartitionKey", "RowKey")]
    class SampleEntity6
    {
        public Guid PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Value { get; set; }
    }

    class SampleEntity7
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string Key { get; set; }

        public int Value { get; set; }
    }

    class SampleEntity8
    {
        public Guid Id { get; set; }

        public string Value { get; set; }
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_searching_by_existing_keys_on_entity_with_keys_defined_by_data_service_key_attribute
    {
        static IQueryable<SampleEntity1> sample;
        static Guid target_partition_key;
        static SampleEntity1 found_entity;

        Establish context = () =>
        {
            target_partition_key = Guid.NewGuid();

            sample = new List<SampleEntity1>
            {
                new SampleEntity1 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v1" },
                new SampleEntity1 { PartitionKey = target_partition_key,  RowKey = "r", Value = "v2" },
                new SampleEntity1 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v3" },
                new SampleEntity1 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v4" }
            }.AsQueryable();
        };

        Because of =
            () => found_entity = sample.FindByKeys<SampleEntity1>(target_partition_key, "r");

        It should_return_entity =
            () => found_entity.ShouldNotBeNull();

        It should_find_entity_with_expected_first_key =
            () => found_entity.PartitionKey.ShouldEqual(target_partition_key);

        It should_find_entity_with_expected_second_key =
            () => found_entity.RowKey.ShouldEqual("r");

        It should_find_entity_with_expected_value =
            () => found_entity.Value.ShouldEqual("v2");
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_searching_by_non_existing_keys_on_entity_with_keys_defined_by_data_service_key_attribute
    {
        static IQueryable<SampleEntity1> sample;
        static SampleEntity1 found_entity;

        Establish context = () =>
        {
            sample = new List<SampleEntity1>
            {
                new SampleEntity1 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v1" },
                new SampleEntity1 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v2" },
                new SampleEntity1 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v3" },
                new SampleEntity1 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v4" }
            }.AsQueryable();
        };

        Because of =
            () => found_entity = sample.FindByKeys<SampleEntity1>(Guid.NewGuid(), "r");

        It should_return_null =
            () => found_entity.ShouldBeNull();
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_searching_on_entity_and_number_of_search_keys_not_matching_number_of_defined_keys
    {
        static IQueryable<SampleEntity1> sample;
        static Exception exception;

        Establish context = () =>
        {
            sample = new List<SampleEntity1>
            {
                new SampleEntity1 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v1" },
                new SampleEntity1 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v2" },
                new SampleEntity1 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v3" },
                new SampleEntity1 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v4" }
            }.AsQueryable();
        };

        Because of =
            () => exception = Catch.Exception(() => sample.FindByKeys<SampleEntity1>(Guid.NewGuid()));

        It should_throw_invalid_operation_exception =
            () => exception.ShouldBeOfType<InvalidOperationException>();

        It should_throw_exception_with_expected_message =
            () => exception.Message.ShouldEqual("Number of key properties doesn't match number of passed values for entity DocaLabs.Storage.Core.Tests.Utils.SampleEntity1.");
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_name_in_data_service_key_attribute_does_not_match_any_property
    {
        static IQueryable<SampleEntity6> sample;
        static Exception exception;

        Establish context = () =>
        {
            sample = new List<SampleEntity6>
            {
                new SampleEntity6 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v1" },
                new SampleEntity6 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v2" },
                new SampleEntity6 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v3" },
                new SampleEntity6 { PartitionKey = Guid.NewGuid(),  RowKey = "r", Value = "v4" }
            }.AsQueryable();
        };

        Because of =
            () => exception = Catch.Exception(() => sample.FindByKeys<SampleEntity6>(Guid.NewGuid()));

        It should_throw_invalid_operation_exception =
            () => exception.ShouldBeOfType<InvalidOperationException>();

        It should_throw_exception_with_expected_message =
            () => exception.Message.ShouldEqual("There is no matching key property UnknownPartitionKey for entity DocaLabs.Storage.Core.Tests.Utils.SampleEntity6.");
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_searching_by_existing_keys_on_entity_with_key_property_derived_from_type_name_plus_id

    {
        static IQueryable<SampleEntity3> sample;
        static Guid target_partition_key;
        static SampleEntity3 found_entity;

        Establish context = () =>
        {
            target_partition_key = Guid.NewGuid();

            sample = new List<SampleEntity3>
            {
                new SampleEntity3 { SampleEntity3Id = Guid.NewGuid(),  Value = "v1" },
                new SampleEntity3 { SampleEntity3Id = target_partition_key,  Value = "v2" },
                new SampleEntity3 { SampleEntity3Id = Guid.NewGuid(),  Value = "v3" },
                new SampleEntity3 { SampleEntity3Id = Guid.NewGuid(),  Value = "v4" }
            }.AsQueryable();
        };

        Because of =
            () => found_entity = sample.FindByKeys<SampleEntity3>(target_partition_key);

        It should_return_entity =
            () => found_entity.ShouldNotBeNull();

        It should_find_entity_with_expected_key =
            () => found_entity.SampleEntity3Id.ShouldEqual(target_partition_key);

        It should_find_entity_with_expected_value =
            () => found_entity.Value.ShouldEqual("v2");
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_searching_by_existing_keys_on_entity_with_key_property_named_id
    {
        static IQueryable<SampleEntity8> sample;
        static Guid target_partition_key;
        static SampleEntity8 found_entity;

        Establish context = () =>
        {
            target_partition_key = Guid.NewGuid();

            sample = new List<SampleEntity8>
            {
                new SampleEntity8 { Id = Guid.NewGuid(),  Value = "v1" },
                new SampleEntity8 { Id = target_partition_key,  Value = "v2" },
                new SampleEntity8 { Id = Guid.NewGuid(),  Value = "v3" },
                new SampleEntity8 { Id = Guid.NewGuid(),  Value = "v4" }
            }.AsQueryable();
        };

        Because of =
            () => found_entity = sample.FindByKeys<SampleEntity8>(target_partition_key);

        It should_return_entity =
            () => found_entity.ShouldNotBeNull();

        It should_find_entity_with_expected_key =
            () => found_entity.Id.ShouldEqual(target_partition_key);

        It should_find_entity_with_expected_value =
            () => found_entity.Value.ShouldEqual("v2");
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_searching_by_existing_key_on_entity_with_key_defined_by_key_attribute
    {
        static IQueryable<SampleEntity2> sample;
        static Guid target_partition_key;
        static SampleEntity2 found_entity;

        Establish context = () =>
        {
            target_partition_key = Guid.NewGuid();

            sample = new List<SampleEntity2>
            {
                new SampleEntity2 { Key = Guid.NewGuid(),  Value = "v1" },
                new SampleEntity2 { Key = target_partition_key,  Value = "v2" },
                new SampleEntity2 { Key = Guid.NewGuid(),  Value = "v3" },
                new SampleEntity2 { Key = Guid.NewGuid(),  Value = "v4" }
            }.AsQueryable();
        };

        Because of =
            () => found_entity = sample.FindByKeys<SampleEntity2>(target_partition_key);

        It should_return_entity =
            () => found_entity.ShouldNotBeNull();

        It should_find_entity_with_expected_key =
            () => found_entity.Key.ShouldEqual(target_partition_key);

        It should_find_entity_with_expected_value =
            () => found_entity.Value.ShouldEqual("v2");
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_searching_by_existing_key_with_different_but_compatible_type
    {
        static IQueryable<SampleEntity7> sample;
        static Guid target_partition_key;
        static SampleEntity7 found_entity;

        Establish context = () =>
        {
            target_partition_key = Guid.NewGuid();

            sample = new List<SampleEntity7>
            {
                new SampleEntity7 { Key = Guid.NewGuid().ToString(),  Value = 1 },
                new SampleEntity7 { Key = target_partition_key.ToString(),  Value = 2 },
                new SampleEntity7 { Key = Guid.NewGuid().ToString(),  Value = 3 },
                new SampleEntity7 { Key = Guid.NewGuid().ToString(),  Value = 4 }
            }.AsQueryable();
        };

        Because of =
            () => found_entity = sample.FindByKeys<SampleEntity7>(target_partition_key);

        It should_return_entity =
            () => found_entity.ShouldNotBeNull();

        It should_find_entity_with_expected_key =
            () => found_entity.Key.ShouldEqual(target_partition_key.ToString());

        It should_find_entity_with_expected_value =
            () => found_entity.Value.ShouldEqual(2);
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_searching_by_existing_key_on_entity_finds_more_than_one_entity
    {
        static IQueryable<SampleEntity2> sample;
        static Guid target_partition_key;
        static Exception exception;

        Establish context = () =>
        {
            target_partition_key = Guid.NewGuid();

            sample = new List<SampleEntity2>
            {
                new SampleEntity2 { Key = target_partition_key,  Value = "v1" },
                new SampleEntity2 { Key = target_partition_key,  Value = "v2" },
                new SampleEntity2 { Key = Guid.NewGuid(),  Value = "v3" },
                new SampleEntity2 { Key = Guid.NewGuid(),  Value = "v4" }
            }.AsQueryable();
        };

        Because of =
            () => exception = Catch.Exception(() => sample.FindByKeys<SampleEntity2>(target_partition_key));

        It should_throw_invalid_operation_exception =
            () => exception.ShouldBeOfType<InvalidOperationException>();

        It should_throw_exception_with_expected_message =
            () => exception.Message.ShouldEqual("The get uniqie query returned more than one entity.");
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_there_are_more_than_one_property_marked_with_key_attribute
    {
        static IQueryable<SampleEntity4> sample;
        static Exception exception;

        Establish context = () =>
        {
            sample = new List<SampleEntity4>
            {
                new SampleEntity4 { PartitionKey = Guid.NewGuid(), RowKey = "r",  Value = "v1" },
                new SampleEntity4 { PartitionKey = Guid.NewGuid(), RowKey = "r",  Value = "v2" },
                new SampleEntity4 { PartitionKey = Guid.NewGuid(), RowKey = "r",  Value = "v3" },
                new SampleEntity4 { PartitionKey = Guid.NewGuid(), RowKey = "r",  Value = "v4" }
            }.AsQueryable();
        };

        Because of =
            () => exception = Catch.Exception(() => sample.FindByKeys<SampleEntity4>(Guid.NewGuid(), "r"));

        It should_throw_invalid_operation_exception =
            () => exception.ShouldBeOfType<InvalidOperationException>();

        It should_throw_exception_with_expected_message =
            () => exception.Message.ShouldEqual("More than one property as markes by KeyAttribute in entity DocaLabs.Storage.Core.Tests.Utils.SampleEntity4, the result maybe unpredictable as reflection doesn't guarantee the order of attributes.");
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_there_is_no_any_property_specified_as_key
    {
        static IQueryable<SampleEntity5> sample;
        static Exception exception;

        Establish context = () =>
        {
            sample = new List<SampleEntity5>
            {
                new SampleEntity5 { Key = Guid.NewGuid(), Value = "v1" },
                new SampleEntity5 { Key = Guid.NewGuid(), Value = "v2" },
                new SampleEntity5 { Key = Guid.NewGuid(), Value = "v3" },
                new SampleEntity5 { Key = Guid.NewGuid(), Value = "v4" }
            }.AsQueryable();
        };

        Because of =
            () => exception = Catch.Exception(() => sample.FindByKeys<SampleEntity5>(Guid.NewGuid()));

        It should_throw_invalid_operation_exception =
            () => exception.ShouldBeOfType<InvalidOperationException>();

        It should_throw_exception_with_expected_message =
            () => exception.Message.ShouldEqual("There is no any key defined for entity DocaLabs.Storage.Core.Tests.Utils.SampleEntity5.");
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_the_source_collection_is_null
    {
        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        static Exception exception;

        Because of =
            () => exception = Catch.Exception(() => ((IQueryable<SampleEntity2>)null).FindByKeys<SampleEntity2>(Guid.NewGuid()));

        It should_throw_invalid_operation_exception =
            () => exception.ShouldBeOfType<ArgumentNullException>();

        It should_throw_exception_with_parameter_name_equal_to_source =
            () => ((ArgumentNullException)exception).ParamName.ShouldEqual("source");
        // ReSharper restore ConditionIsAlwaysTrueOrFalse
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_the_entity_type_is_null
    {
        static IQueryable<SampleEntity2> sample;
        static Exception exception;

        Establish context = () =>
        {
            sample = new List<SampleEntity2>
            {
                new SampleEntity2 { Key = Guid.NewGuid(), Value = "v1" },
                new SampleEntity2 { Key = Guid.NewGuid(), Value = "v2" },
                new SampleEntity2 { Key = Guid.NewGuid(), Value = "v3" },
                new SampleEntity2 { Key = Guid.NewGuid(), Value = "v4" }
            }.AsQueryable();
        };

        Because of =
            () => exception = Catch.Exception(() => sample.FindByKeys(null, Guid.NewGuid()));

        It should_throw_invalid_operation_exception =
            () => exception.ShouldBeOfType<ArgumentNullException>();

        It should_throw_exception_with_parameter_name_equal_to_source =
            () => ((ArgumentNullException)exception).ParamName.ShouldEqual("entityType");
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_the_passed_key_value_is_null
    {
        static IQueryable<SampleEntity2> sample;
        static Exception exception;

        Establish context = () =>
        {
            sample = new List<SampleEntity2>
            {
                new SampleEntity2 { Key = Guid.NewGuid(), Value = "v1" },
                new SampleEntity2 { Key = Guid.NewGuid(), Value = "v2" },
                new SampleEntity2 { Key = Guid.NewGuid(), Value = "v3" },
                new SampleEntity2 { Key = Guid.NewGuid(), Value = "v4" }
            }.AsQueryable();
        };

        Because of =
            () => exception = Catch.Exception(() => sample.FindByKeys<SampleEntity2>(null));

        It should_throw_invalid_operation_exception =
            () => exception.ShouldBeOfType<ArgumentNullException>();

        It should_throw_exception_with_parameter_name_equal_to_source =
            () => ((ArgumentNullException)exception).ParamName.ShouldEqual("keyValues");
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_the_passed_array_of_key_value_is_empty
    {
        static IQueryable<SampleEntity2> sample;
        static Exception exception;

        Establish context = () =>
        {
            sample = new List<SampleEntity2>
            {
                new SampleEntity2 { Key = Guid.NewGuid(), Value = "v1" },
                new SampleEntity2 { Key = Guid.NewGuid(), Value = "v2" },
                new SampleEntity2 { Key = Guid.NewGuid(), Value = "v3" },
                new SampleEntity2 { Key = Guid.NewGuid(), Value = "v4" }
            }.AsQueryable();
        };

        Because of =
            () => exception = Catch.Exception(() => sample.FindByKeys<SampleEntity2>(new object[0]));

        It should_throw_invalid_operation_exception =
            () => exception.ShouldBeOfType<ArgumentNullException>();

        It should_throw_exception_with_parameter_name_equal_to_source =
            () => ((ArgumentNullException)exception).ParamName.ShouldEqual("keyValues");
    }

    [Subject(typeof(QueryableEntityFinder)), UnitTestTag]
    class when_converter_is_set_to_other_than_default
    {
        class MyException : Exception
        {
        }

        static IQueryable<SampleEntity7> sample;
        static Guid target_partition_key;
        static Mock<CustomConverter> mock_converter;
        static Exception exception;

        Cleanup after_each = () => QueryableEntityFinder.Converter = null;

        Establish context = () =>
        {
            mock_converter = new Mock<CustomConverter>();
            mock_converter.Setup(x => x.ChangeType<string>(Moq.It.IsAny<object>())).Throws(new MyException());
            mock_converter.Setup(x => x.ChangeType(Moq.It.IsAny<object>(), typeof(string))).Throws(new MyException());

            target_partition_key = Guid.NewGuid();

            sample = new List<SampleEntity7>
            {
                new SampleEntity7 { Key = Guid.NewGuid().ToString(),  Value = 1 },
                new SampleEntity7 { Key = target_partition_key.ToString(),  Value = 2 },
                new SampleEntity7 { Key = Guid.NewGuid().ToString(),  Value = 3 },
                new SampleEntity7 { Key = Guid.NewGuid().ToString(),  Value = 4 }
            }.AsQueryable();
        };

        Because of = () => exception = Catch.Exception(() =>
        {
            QueryableEntityFinder.Converter = mock_converter.Object;
            sample.FindByKeys<SampleEntity7>(target_partition_key);
        });

        It should_use_provided_converter =
            () => exception.ShouldBeOfType<MyException>();
    }
}