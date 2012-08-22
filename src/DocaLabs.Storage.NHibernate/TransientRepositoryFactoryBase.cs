using DocaLabs.Storage.Core;
using NHibernate;

namespace DocaLabs.Storage.NHibernate
{
    /// <summary>
    /// Provides base class for TransientRepositoryFactory implementations to create an instance of a transient (repository which each time uses a new database connection).
    /// using the specified session factory and connections string. The side effect is that there will be only one repository per connection.
    /// </summary>
    public abstract class TransientRepositoryFactoryBase : TransientRepositoryFactory
    {
        /// <summary>
        /// Gets an underlying session factory.
        /// </summary>
        protected ISessionFactory SessionFactory { get; set; }

        /// <summary>
        /// Initializes an instance of the TransientRepositoryFactoryBase using specified session factory.
        /// </summary>
        protected TransientRepositoryFactoryBase(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
        }

        /// <summary>
        /// Implements transient repository.
        /// </summary>
        protected class TransientRepository<TEntity> : Repository<TEntity>, ITransientRepository<TEntity>
            where TEntity : class , IEntity
        {
            /// <summary>
            /// Initializes a new instance of the TransientRepository using specified session manager.
            /// </summary>
            public TransientRepository(ISessionManager sessionManager)
                : base(sessionManager)
            {
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting resources.
            /// </summary>
            public void Dispose()
            {
                SessionManager.Dispose();
            }
        }
    }
}