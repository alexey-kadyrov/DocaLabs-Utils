using System;
using System.Data.Common;
using DocaLabs.Storage.Core.Partitioning;
using Machine.Specifications;
using Machine.Specifications.Annotations;
using Moq;
using It=Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Tests.Partitioning
{
    [Subject(typeof(PartitionProxy))]
    class when_partition_proxy_is_used
    {
        static PartitionProxy proxy;
        static Mock<IPartitionConnectionProvider> mock_partition_provider;
        static Mock<IPartitionKeyProvider> mock_partition_key_provider;
        static Mock<DbConnection> mock_connection;
        static Mock<IDatabaseConnection> mock_connection_wrapper; 
        static Guid partition_key;

        [UsedImplicitly] Establish context = () =>
        {
            partition_key = Guid.Parse("9271F92B-0D1D-4E93-BC84-2E990A65DBFF");

            mock_connection = new Mock<DbConnection>();

            mock_connection_wrapper = new Mock<IDatabaseConnection>();
            mock_connection_wrapper.Setup(x => x.Connection).Returns(mock_connection.Object);

            mock_partition_key_provider = new Mock<IPartitionKeyProvider>();
            mock_partition_key_provider.Setup(x => x.GetPartitionKey())
                .Returns(partition_key);

            mock_partition_provider = new Mock<IPartitionConnectionProvider>();
            mock_partition_provider.Setup(x => x.GetConnection(partition_key))
                .Returns(mock_connection_wrapper.Object);
        };

        Because of =
            () => proxy = new PartitionProxy(mock_partition_key_provider.Object, mock_partition_provider.Object);

        It should_return_specified_partition_key_provider =
            () => proxy.PartitionKeyProvider.ShouldBeTheSameAs(mock_partition_key_provider.Object);

        It should_return_specified_partition_provider =
            () => proxy.PartitionConnectionProvider.ShouldBeTheSameAs(mock_partition_provider.Object);

        It should_use_supplied_providers =
            () => proxy.GetConnection().ShouldNotBeTheSameAs(mock_connection);
    }

    [Subject(typeof(PartitionProxy))]
    class when_partition_proxy_is_newed_with_null_partition_key_provider
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new PartitionProxy(null, new Mock<IPartitionConnectionProvider>().Object));

        It should_throw_argumant_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_partition_key_provider =
            () => ((ArgumentNullException) actual_exception).ParamName.ShouldEqual("partitionKeyProvider");
    }

    [Subject(typeof(PartitionProxy))]
    class when_partition_proxy_is_newed_with_null_partition_provider
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new PartitionProxy(new Mock<IPartitionKeyProvider>().Object, null));

        It should_throw_argumant_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_partition_provider =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("partitionProvider");
    }
}
