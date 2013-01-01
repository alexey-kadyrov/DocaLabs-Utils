using System;
using System.Collections.Generic;
using DocaLabs.Storage.Core.Repositories;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Scenarios
{
    interface IRepositoryScenarioProvider : IDisposable
    {
        IQueryableRepository<TEntity> CreateRepository<TEntity>() where TEntity : class;
        TEntity GetEntity<TEntity>(object id) where TEntity : class;
        void Save<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    }
}
