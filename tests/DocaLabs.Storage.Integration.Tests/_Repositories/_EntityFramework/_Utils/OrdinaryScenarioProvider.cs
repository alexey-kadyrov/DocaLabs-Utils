using DocaLabs.EntityFrameworkStorage;
using DocaLabs.Storage.Integration.Tests._Repositories._Scenarios;
using DocaLabs.Testing.Common;

namespace DocaLabs.Storage.Integration.Tests._Repositories._EntityFramework._Utils
{
    class OrdinaryScenarioProvider : ScenarioProviderBase
    {
        public OrdinaryScenarioProvider()
        {
            var contextFactory = new DbContextFactory<RepositoryAggregateRoot<Book>>(MsSqlHelper.EfConnectionStringName);
            Session = new RepositorySession(contextFactory);
        }
    }
}
