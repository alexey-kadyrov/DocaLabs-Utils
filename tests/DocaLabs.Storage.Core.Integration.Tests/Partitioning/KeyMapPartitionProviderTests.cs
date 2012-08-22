using System;
using System.Data;
using System.Transactions;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.Database;
using DocaLabs.Testing.Common.MSpec;
using DocaLabs.Testing.Common.Mathematics;
using Machine.Specifications;
using Machine.Specifications.Annotations;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Integration.Tests.Partitioning
{
    /// <remarks>
    /// IAssemblyContext is not reliable running under Resharper's test runner
    /// see: https://github.com/machine/machine.specifications/issues/17
    /// </remarks>
    static class KeyMapPartitionProviderTestsSetup
    {
        public const string PartitionMapConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsKeyMapPartitionProviderTests;Integrated Security=SSPI;";
        public const string NonExistingPartitionMapConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=NonExistingDocaLabsKeyMapPartitionProviderTests;Integrated Security=SSPI;";

        static bool IsInitialized { get; set; }

        public static void Setup()
        {
            if (IsInitialized)
                return;

            AppDomain.CurrentDomain.DomainUnload += HandleDomainUnload;

            MsSqlDatabaseBuilder.Build(PartitionMapConnectionString,
                        @"Partitioning/1-base-partitioning-create-tables-and-sprocs.sql",
                        @"Partitioning/2-extended-partitioning-create-tables-and-sprocs.sql",
                        @"Partitioning/create-dummy-table.sql");

            IsInitialized = true;
        }

        static void HandleDomainUnload(object sender, EventArgs e)
        {
            IsInitialized = false;

            AppDomain.CurrentDomain.DomainUnload -= HandleDomainUnload;

            MsSqlDatabaseBuilder.Drop(PartitionMapConnectionString);
        }
    }

    class KeyMapPartitionProviderTestsContext
    {
        [UsedImplicitly] Establish before_each = 
            () => KeyMapPartitionProviderTestsSetup.Setup();
    }

    [Subject(typeof(KeyMapPartitionProvider)), IntegrationTag]
    class when_key_map_partition_provider_is_newed_with_7_auto_asign_partitions_and_two_manual_partitons : KeyMapPartitionProviderTestsContext
    {
        static KeyMapPartitionProvider provider;
        static Guid partition_key_in_first_manual_partition;
        static Guid partition_key_in_second_manual_partition;

        Establish context = () =>
        {
            partition_key_in_first_manual_partition = Guid.NewGuid();
            partition_key_in_second_manual_partition = Guid.NewGuid();
        };

        Because of = () =>
        {
            // the test is wrapped in external 'dummy' transaction which should not affect the transaction used by the provider itself.
            // as SQLEXPRESS doesn't support MSDTC so if the provider doesn't create it's own 'local' transaction the test would fail.
            // It's important for Azure Sql as it doesn't support distributed transactions.
            using (new TransactionScope())
            using (var dummyConnection = new DbConnectionString(KeyMapPartitionProviderTestsSetup.PartitionMapConnectionString).CreateDbConnection())
            using (var dummyCommand = dummyConnection.CreateCommand())
            {
                dummyCommand.CommandType = CommandType.Text;
                dummyCommand.CommandText = "INSERT INTO [DummyTable]([Value]) VALUES(1)";

                dummyConnection.Open();
                dummyCommand.ExecuteNonQuery();

                provider =
                    new KeyMapPartitionProvider(
                        new DbConnectionString(KeyMapPartitionProviderTestsSetup.PartitionMapConnectionString));

                var autoAssignParttionCount = PartitionValidator.PartitionConnectionStrings.Length;

                // add auto assign partitions
                for (var i = 0; i < autoAssignParttionCount; i++)
                    provider.AddNewAutoAssignPartition(i, PartitionValidator.PartitionConnectionStrings[i]);

                // add manual partitions
                provider.AddNewManualPartition(autoAssignParttionCount + 1, PartitionValidator.MakePartitionDbConnectionString(autoAssignParttionCount + 1));
                provider.AddNewManualPartition(autoAssignParttionCount + 2, PartitionValidator.MakePartitionDbConnectionString(autoAssignParttionCount + 2));

                // add partition keys to manual partition
                provider.AddKeyToManualPartition(autoAssignParttionCount + 1, partition_key_in_first_manual_partition);
                provider.AddKeyToManualPartition(autoAssignParttionCount + 2, partition_key_in_second_manual_partition);
            }
        };

        It should_auto_assign_partiton_and_return_connection_string_for_all_passed_partition_keys_igoring_ambient_transaction_also_should_have_excellent_distribution = () =>
        {
            // the test is wrapped in external 'dummy' transaction which should not affect the transaction used by the provider itself.
            // as SQLEXPRESS doesn't support MSDTC so if the provider doesn't create it's own 'local' transaction the test would fail.
            // It's important for Azure Sql as it doesn't support distributed transactions.
            using (new TransactionScope())
            using (var dummyConnection = new DbConnectionString(KeyMapPartitionProviderTestsSetup.PartitionMapConnectionString).CreateDbConnection())
            using (var dummyCommand = dummyConnection.CreateCommand())
            {
                dummyCommand.CommandType = CommandType.Text;
                dummyCommand.CommandText = "INSERT INTO [DummyTable]([Value]) VALUES(1)";

                dummyConnection.Open();
                dummyCommand.ExecuteNonQuery();

                provider.ShouldGetPartitionsForKeys(short.MaxValue/2).ShouldHaveUniformDistributionDeviationProbability(UniformDistributionQuality.Excellent);
            }
        };

        It should_get_connection_for_partiton_key_from_first_manual_partiton =
            () => provider.GetConnection(partition_key_in_first_manual_partition).Connection.ExtractPartitionFromConnection().ShouldEqual(PartitionValidator.PartitionConnectionStrings.Length + 1);

        It should_get_connection_for_partiton_key_from_second_manual_partiton =
            () => provider.GetConnection(partition_key_in_second_manual_partition).Connection.ExtractPartitionFromConnection().ShouldEqual(PartitionValidator.PartitionConnectionStrings.Length + 2);
    }

    [Subject(typeof(KeyMapPartitionProvider)), IntegrationTag]
    class when_add_new_manual_partition_is_called_with_partition_less_than_zero : KeyMapPartitionProviderTestsContext
    {
        static KeyMapPartitionProvider provider;
        static Exception actual_exception;

        Establish context =
            () => provider = new KeyMapPartitionProvider(new DbConnectionString(KeyMapPartitionProviderTestsSetup.PartitionMapConnectionString));

        Because of =
            () => actual_exception = Catch.Exception(() => provider.AddNewManualPartition(-1, PartitionValidator.MakePartitionDbConnectionString(100)));

        It should_throw_argument_out_of_range_exception =
            () => actual_exception.ShouldBeOfType<ArgumentOutOfRangeException>();

        It should_report_partition_argument =
            () => ((ArgumentOutOfRangeException)actual_exception).ParamName.ShouldEqual("partition");
    }

    [Subject(typeof(KeyMapPartitionProvider)), IntegrationTag]
    class when_add_new_manual_partition_is_called_with_null_connection_string : KeyMapPartitionProviderTestsContext
    {
        static KeyMapPartitionProvider provider;
        static Exception actual_exception;

        Establish context =
            () => provider = new KeyMapPartitionProvider(new DbConnectionString(KeyMapPartitionProviderTestsSetup.PartitionMapConnectionString));

        Because of =
            () => actual_exception = Catch.Exception(() => provider.AddNewManualPartition(100, null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_partition_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("connectionString");
    }

    [Subject(typeof(KeyMapPartitionProvider)), IntegrationTag]
    class when_add_new_auto_assign_partition_is_called_with_partition_less_than_zero : KeyMapPartitionProviderTestsContext
    {
        static KeyMapPartitionProvider provider;
        static Exception actual_exception;

        Establish context =
            () => provider = new KeyMapPartitionProvider(new DbConnectionString(KeyMapPartitionProviderTestsSetup.PartitionMapConnectionString));

        Because of =
            () => actual_exception = Catch.Exception(() => provider.AddNewAutoAssignPartition(-1, PartitionValidator.MakePartitionDbConnectionString(100)));

        It should_throw_argument_out_of_range_exception =
            () => actual_exception.ShouldBeOfType<ArgumentOutOfRangeException>();

        It should_report_partition_argument =
            () => ((ArgumentOutOfRangeException)actual_exception).ParamName.ShouldEqual("partition");
    }

    [Subject(typeof(KeyMapPartitionProvider)), IntegrationTag]
    class when_add_new_auto_assign_partition_is_called_with_null_connection_string : KeyMapPartitionProviderTestsContext
    {
        static KeyMapPartitionProvider provider;
        static Exception actual_exception;

        Establish context =
            () => provider = new KeyMapPartitionProvider(new DbConnectionString(KeyMapPartitionProviderTestsSetup.PartitionMapConnectionString));

        Because of =
            () => actual_exception = Catch.Exception(() => provider.AddNewAutoAssignPartition(100, null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_partition_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("connectionString");
    }

    [Subject(typeof(KeyMapPartitionProvider)), IntegrationTag]
    class when_add_key_to_manual_partition_is_called_with_partition_less_than_zero : KeyMapPartitionProviderTestsContext
    {
        static KeyMapPartitionProvider provider;
        static Exception actual_exception;

        Establish context =
            () => provider = new KeyMapPartitionProvider(new DbConnectionString(KeyMapPartitionProviderTestsSetup.PartitionMapConnectionString));

        Because of =
            () => actual_exception = Catch.Exception(() => provider.AddKeyToManualPartition(-1, Guid.NewGuid()));

        It should_throw_argument_out_of_range_exception =
            () => actual_exception.ShouldBeOfType<ArgumentOutOfRangeException>();

        It should_report_partition_argument =
            () => ((ArgumentOutOfRangeException)actual_exception).ParamName.ShouldEqual("partition");
    }

    [Subject(typeof(KeyMapPartitionProvider)), IntegrationTag]
    class when_key_map_partition_provider_is_newed_with_null_connection_string_argument
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new KeyMapPartitionProvider(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_partition_map_connection_string_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("partitionMapConnectionString");
    }

    [Subject(typeof(KeyMapPartitionProvider)), IntegrationTag]
    class when_adding_new_manual_partiton_there_are_problems_with_connection_to_database_as_database_does_not_exist
    {
        static KeyMapPartitionProvider provider;
        static Exception actual_exception;

        Establish context =
            () => provider = new KeyMapPartitionProvider(new DbConnectionString(KeyMapPartitionProviderTestsSetup.NonExistingPartitionMapConnectionString));

        Because of =
            () => actual_exception = Catch.Exception(() => provider.AddNewManualPartition(100, PartitionValidator.MakePartitionDbConnectionString(100)));

        It should_throw_partition_exception =
            () => actual_exception.ShouldBeOfType<PartitionException>();

        It should_wrap_actual_inner_exception =
            () => actual_exception.InnerException.ShouldNotBeNull();
    }

    [Subject(typeof(KeyMapPartitionProvider)), IntegrationTag]
    class when_adding_new_auto_assign_partiton_there_are_problems_with_connection_to_database_as_database_does_not_exist
    {
        static KeyMapPartitionProvider provider;
        static Exception actual_exception;

        Establish context =
            () => provider = new KeyMapPartitionProvider(new DbConnectionString(KeyMapPartitionProviderTestsSetup.NonExistingPartitionMapConnectionString));

        Because of =
            () => actual_exception = Catch.Exception(() => provider.AddNewAutoAssignPartition(100, PartitionValidator.MakePartitionDbConnectionString(100)));

        It should_throw_partition_exception =
            () => actual_exception.ShouldBeOfType<PartitionException>();

        It should_wrap_actual_inner_exception =
            () => actual_exception.InnerException.ShouldNotBeNull();
    }

    [Subject(typeof(KeyMapPartitionProvider)), IntegrationTag]
    class when_adding_key_to_manual_partiton_there_are_problems_with_connection_to_database_as_database_does_not_exist
    {
        static KeyMapPartitionProvider provider;
        static Exception actual_exception;

        Establish context =
            () => provider = new KeyMapPartitionProvider(new DbConnectionString(KeyMapPartitionProviderTestsSetup.NonExistingPartitionMapConnectionString));

        Because of =
            () => actual_exception = Catch.Exception(() => provider.AddKeyToManualPartition(100, Guid.NewGuid()));

        It should_throw_partition_exception =
            () => actual_exception.ShouldBeOfType<PartitionException>();

        It should_wrap_actual_inner_exception =
            () => actual_exception.InnerException.ShouldNotBeNull();
    }

    [Subject(typeof(KeyMapPartitionProvider)), IntegrationTag]
    class when_getting_connection_string_there_are_problems_with_connection_to_database_as_database_does_not_exist
    {
        static KeyMapPartitionProvider provider;
        static Exception actual_exception;

        Establish context =
            () => provider = new KeyMapPartitionProvider(new DbConnectionString(KeyMapPartitionProviderTestsSetup.NonExistingPartitionMapConnectionString));

        Because of =
            () => actual_exception = Catch.Exception(() => provider.GetConnection(Guid.NewGuid()));

        It should_throw_partition_exception =
            () => actual_exception.ShouldBeOfType<PartitionException>();

        It should_wrap_actual_inner_exception =
            () => actual_exception.InnerException.ShouldNotBeNull();
    }

    [Subject(typeof(KeyMapPartitionProvider)), IntegrationTag]
    class when_getting_connection_string_from_database_that_has_non_populated_partition_information : KeyMapPartitionProviderTestsContext
    {
        public const string IncompletePartitionMapConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsIncompleteKeyMapPartitionProviderTests;Integrated Security=SSPI;";

        static KeyMapPartitionProvider provider;
        static Exception actual_exception;

        Cleanup after =
            () => MsSqlDatabaseBuilder.Drop(IncompletePartitionMapConnectionString);

        Establish context = () =>
        {
            MsSqlDatabaseBuilder.Build(IncompletePartitionMapConnectionString,
                        @"Partitioning/1-base-partitioning-create-tables-and-sprocs.sql",
                        @"Partitioning/2-extended-partitioning-create-tables-and-sprocs.sql");
            provider = new KeyMapPartitionProvider(new DbConnectionString(IncompletePartitionMapConnectionString));
        };

        Because of =
            () => actual_exception = Catch.Exception(() => provider.GetConnection(Guid.NewGuid()));

        It should_throw_partition_exception =
            () => actual_exception.ShouldBeOfType<PartitionException>();

        It should_wrap_actual_inner_exception =
            () => actual_exception.InnerException.ShouldNotBeNull();
    }
}
