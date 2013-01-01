using System.Collections.Generic;
using System.Transactions;
using DocaLabs.NHibernateStorage;
using DocaLabs.Storage.Core.Repositories;
using DocaLabs.Storage.Integration.Tests._Repositories._Scenarios;
using NHibernate;

namespace DocaLabs.Storage.Integration.Tests._Repositories._NHibernate._Utils
{
    class OrdinaryScenarioProvider : IRepositoryScenarioProvider
    {
        readonly ISessionFactory _sessionFactory;
        readonly INHibernateRepositorySession _sessionManager;

        public OrdinaryScenarioProvider()
        {
            _sessionFactory = NHibernateSessionFactoryBuilder.Build();
            _sessionManager = new RepositorySession(_sessionFactory);
        }

        public void Dispose()
        {
            _sessionManager.Dispose();
            _sessionFactory.Dispose();
        }

        public IQueryableRepository<TEntity> CreateRepository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(_sessionManager);
        }

        public TEntity GetEntity<TEntity>(object id) where TEntity : class
        {
            using (var session = _sessionFactory.OpenSession())
            {
                return session.Get<TEntity>(id);
            }
        }

        public void Save<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            using (var session = _sessionFactory.OpenSession())
            {
                foreach (var entity in entities)
                {
                    session.Save(entity);
                }

                scope.Complete();
            }
        }
    }
}
