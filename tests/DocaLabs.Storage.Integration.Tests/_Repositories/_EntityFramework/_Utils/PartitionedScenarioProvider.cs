using DocaLabs.EntityFrameworkStorage;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Integration.Tests._Repositories._Scenarios;
using DocaLabs.Storage.Integration.Tests._Utils;

namespace DocaLabs.Storage.Integration.Tests._Repositories._EntityFramework._Utils
{
    class PartitionedScenarioProvider : ScenarioProviderBase
    {
        public PartitionedScenarioProvider()
        {
            var contextFactory = new PartitionedDbContextFactory<RepositoryAggregateRoot<Book>>(new DatabaseConnection(new DatabaseConnectionString(MsSqlHelper.ConnectionStringSettings)));
            Session = new RepositorySession(contextFactory);
        }
    }
}
