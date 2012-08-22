using System;
using DocaLabs.Storage.Azure.Tables;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Azure.Tests.Tables
{
    class AzureTableEntityUnderTest : AzureTableEntity
    {
        public AzureTableEntityUnderTest()
        {
        }

        public AzureTableEntityUnderTest(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        public AzureTableEntityUnderTest(Guid partitionKey, Guid rowKey)
            : base(partitionKey, rowKey)
        {
        }
    }

    [Subject(typeof(AzureTableEntity)), UnitTestTag]
    class when_azure_table_entity_is_newed_using_default_constractor
    {
        static AzureTableEntity entity;

        Because of =
            () => entity = new AzureTableEntityUnderTest();

        It should_set_partition_key_to_null =
            () => entity.PartitionKey.ShouldBeNull();

        It should_set_row_key_to_null =
            () => entity.RowKey.ShouldBeNull();
    }

    [Subject(typeof(AzureTableEntity)), UnitTestTag]
    class when_azure_table_entity_is_newed_using_overload_constractor_with_string_arguments
    {
        static AzureTableEntity entity;

        Because of =
            () => entity = new AzureTableEntityUnderTest("partition-key", "row-key");

        It should_set_partition_key_to_null =
            () => entity.PartitionKey.ShouldEqual("partition-key");

        It should_set_row_key_to_null =
            () => entity.RowKey.ShouldEqual("row-key");
    }

    [Subject(typeof(AzureTableEntity)), UnitTestTag]
    class when_azure_table_entity_is_newed_using_overload_constractor_with_guid_arguments
    {
        static AzureTableEntity entity;
        static Guid partition_key;
        static Guid row_key;

        Establish context = () =>
        {
            partition_key = Guid.Parse("9271F92B-0D1D-4E93-BC84-2E990A65DBFF");
            row_key = Guid.Parse("40CC4B98-1B59-49FE-8E0D-5B9DA90C3E0D");
        };

        Because of =
            () => entity = new AzureTableEntityUnderTest(partition_key, row_key);

        It should_set_partition_key_to_null =
            () => entity.PartitionKey.ShouldEqual(partition_key.ToString());

        It should_set_row_key_to_null =
            () => entity.RowKey.ShouldEqual(row_key.ToString());
    }
}
