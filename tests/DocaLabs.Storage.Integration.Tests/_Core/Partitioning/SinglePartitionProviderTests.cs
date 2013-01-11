using System;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Testing.Common;
using Machine.Specifications;

namespace DocaLabs.Storage.Integration.Tests._Core.Partitioning
{
    [Subject(typeof(SinglePartitionProvider))]
    class when_single_partition_provider_is_newed_with_specified_db_connection_string
    {
        static DatabaseConnectionString conection_string;
        static SinglePartitionProvider provider;
        static IDatabaseConnection connection;

        Establish context = () =>
        {
            conection_string = new DatabaseConnectionString(MsSqlHelper.ConnectionStringSettings);
            provider = new SinglePartitionProvider(conection_string);
        };

        Because of =
            () => connection = provider.GetConnection(Guid.NewGuid());

        It should_always_return_connection_using_configured_connection_string_regardless_of_partition_key =
            () => connection.Connection.ConnectionString.ShouldEqual(conection_string.ConnectionString);
    }

    [Subject(typeof(SinglePartitionProvider))]
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
