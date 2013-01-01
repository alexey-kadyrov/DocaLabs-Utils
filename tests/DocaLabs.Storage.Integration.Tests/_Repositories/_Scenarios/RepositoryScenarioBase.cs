using System;
using DocaLabs.Storage.Core.Repositories;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Scenarios
{
    abstract class RepositoryScenarioBase<TScenarioPrivider> : IDisposable
        where TScenarioPrivider : IRepositoryScenarioProvider, new()
    {
        protected TScenarioPrivider ScenarioProvider { get; private set; }
        public IQueryableRepository<Book> Books { get; private set; }

        protected RepositoryScenarioBase()
        {
            ScenarioProvider = new TScenarioPrivider();
            Books = ScenarioProvider.CreateRepository<Book>();
        }

        public void Dispose()
        {
            ScenarioProvider.Dispose();
        }

        public Book GetPersistedBook(Guid id)
        {
            return ScenarioProvider.GetEntity<Book>(id);
        }
    }
}
