using System;
using System.Data;
using System.Transactions;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.Database;
using DocaLabs.Testing.Common.MSpec;
using DocaLabs.Testing.Common.Mathematics;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Integration.Tests.Partitioning
{
    [Subject(typeof(HashedPartitionProvider)), IntegrationTag]
    class when_hashed_partition_provider_is_newed
    {
        public const string PartitionMapConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsKeyMapPartitionProviderTests;Integrated Security=SSPI;";

        static HashedPartitionProvider provider;

        Cleanup after =
            () => MsSqlDatabaseBuilder.Drop(PartitionMapConnectionString);

        Establish context = () =>
        {
            MsSqlDatabaseBuilder.Build(PartitionMapConnectionString, 
                @"Partitioning/1-base-partitioning-create-tables-and-sprocs.sql",
                @"Partitioning/create-dummy-table.sql");

            for (var i = 0; i < PartitionValidator.PartitionConnectionStrings.Length; i++)
                MsSqlDatabaseBuilder.ExecuteNonQuery(PartitionMapConnectionString, String.Format("EXEC [AddConnectionString] {0}, '{1}', '{2}'", i, PartitionValidator.PartitionConnectionStrings[i].ProviderName, PartitionValidator.PartitionConnectionStrings[i].ConnectionString));
        };

        Because of = () =>
        {
            // the test is wrapped in external 'dummy' transaction which should not affect the transaction used by the provider itself.
            // as SQLEXPRESS doesn't support MSDTC so if the provider doesn't create it's own 'local' transaction the test would fail.
            // It's important for Azure Sql as it doesn't support distributed transactions.
            using (new TransactionScope())
            using (var dummyConnection = new DbConnectionString(PartitionMapConnectionString).CreateDbConnection())
            using (var dummyCommand = dummyConnection.CreateCommand())
            {
                dummyCommand.CommandType = CommandType.Text;
                dummyCommand.CommandText = "INSERT INTO [DummyTable]([Value]) VALUES(1)";

                dummyConnection.Open();
                dummyCommand.ExecuteNonQuery();

                provider = new HashedPartitionProvider(new DbConnectionString(PartitionMapConnectionString));
            }
        };

        It should_contain_preconfigured_number_of_partitons =
            () => provider.PartitionCount.ShouldEqual(PartitionValidator.PartitionConnectionStrings.Length);

        It should_return_connection_string_for_all_passed_partition_keys_and_should_have_excellent_distribution =
            () => provider.ShouldGetPartitionsForKeys(Int16.MaxValue).ShouldHaveUniformDistributionDeviationProbability(UniformDistributionQuality.Excellent);
    }

    [Subject(typeof(HashedPartitionProvider)), IntegrationTag]
    class when_hashed_partition_provider_is_newed_with_null_connection_string_argument
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new HashedPartitionProvider(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_partition_map_connection_string_argument =
            () => ((ArgumentNullException) actual_exception).ParamName.ShouldEqual("partitionMapConnectionString");
    }

    [Subject(typeof(HashedPartitionProvider)), IntegrationTag]
    class when_hashed_partition_provider_is_newed_with_wrong_connection_string_argument
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new HashedPartitionProvider(new DbConnectionString(Guid.NewGuid().ToString("N"))));

        It should_throw_partition_exception =
            () => actual_exception.ShouldBeOfType<PartitionException>();

        It should_wrap_actual_inner_exception =
            () => actual_exception.InnerException.ShouldNotBeNull();
    }
}
