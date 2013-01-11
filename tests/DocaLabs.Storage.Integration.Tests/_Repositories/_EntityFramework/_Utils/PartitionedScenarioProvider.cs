using DocaLabs.EntityFrameworkStorage;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Integration.Tests._Repositories._Scenarios;
using DocaLabs.Testing.Common;
using Moq;

namespace DocaLabs.Storage.Integration.Tests._Repositories._EntityFramework._Utils
{
    class PartitionedScenarioProvider : ScenarioProviderBase
    {
        readonly Mock<IPartitionProxy> _partitionProxy;

        public PartitionedScenarioProvider()
        {
            _partitionProxy = new Mock<IPartitionProxy>();
            _partitionProxy.Setup(x => x.GetConnection())
                .Returns(() => new DatabaseConnection(new DatabaseConnectionString(MsSqlHelper.ConnectionStringSettings)));

            var contextFactory = new PartitionedDbContextFactory<RepositoryAggregateRoot<Book>>(_partitionProxy.Object);

            Session = new RepositorySession(contextFactory);
        }
    }
}
