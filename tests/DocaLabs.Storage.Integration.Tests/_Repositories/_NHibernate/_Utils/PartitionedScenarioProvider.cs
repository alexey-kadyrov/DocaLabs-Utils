using System.Collections.Generic;
using System.Transactions;
using DocaLabs.NHibernateStorage;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Repositories;
using DocaLabs.Storage.Integration.Tests._Repositories._Scenarios;
using DocaLabs.Storage.Integration.Tests._Utils;
using NHibernate;

namespace DocaLabs.Storage.Integration.Tests._Repositories._NHibernate._Utils
{
    class PartitionedScenarioProvider : IRepositoryScenarioProvider
    {
        readonly ISessionFactory _sessionFactory;
        readonly IDatabaseConnection _databaseConnection;
        readonly INHibernateRepositorySession _sessionManager;

        public PartitionedScenarioProvider()
        {
            _sessionFactory = NHibernateSessionFactoryBuilder.Build();
            _databaseConnection = new DatabaseConnection(new DatabaseConnectionString(MsSqlHelper.ConnectionStringSettings));
            _sessionManager = new PartitionedRepositorySession(_sessionFactory, _databaseConnection);
        }

        public void Dispose()
        {
            _sessionManager.Dispose();
            _databaseConnection.Dispose();
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
