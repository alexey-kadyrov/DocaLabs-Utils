using System;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Integration.Tests.Partitioning
{
    [Subject(typeof(SinglePartitionProvider)), IntegrationTag]
    class when_single_partition_provider_is_newed_with_specified_db_connection_string
    {
        static DbConnectionString conection_string;
        static SinglePartitionProvider provider;
        static IDbConnectionWrapper connection;

        Establish context = () =>
        {
            conection_string = new DbConnectionString(@"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsTests;Integrated Security=SSPI;");
            provider = new SinglePartitionProvider(conection_string);
        };

        Because of =
            () => connection = provider.GetConnection(Guid.NewGuid());

        It should_always_return_connection_using_configured_connection_string_regardless_of_partition_key =
            () => connection.Connection.ConnectionString.ShouldEqual(conection_string.ConnectionString);
    }

    [Subject(typeof(SinglePartitionProvider)), IntegrationTag]
    class when_single_partition_provider_is_newed_with_null_connection_string_argument
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new SinglePartitionProvider(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_partition_map_connection_string_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("connectionString");
    }
}
