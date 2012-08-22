using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Provides base class for TransientRepositoryFactory implementations to create an instance of a transient (repository which each time uses a new database connection).
    /// using the specified session factory and connections string. The side effect is that there will be only one repository per connection.
    /// </summary>
    public abstract class TransientRepositoryFactoryBase : TransientRepositoryFactory
    {
        /// <summary>
        /// Implements transient repository.
        /// </summary>
        protected class TransientRepository<TEntity> : Repository<TEntity>, ITransientRepository<TEntity>
            where TEntity : class , IEntity
        {
            /// <summary>
            /// Initializes a new instance of the TransientRepository using specified session manager.
            /// </summary>
            public TransientRepository(IDbConnectionManager connectionManager)
                : base(connectionManager)
            {
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting resources.
            /// </summary>
            public void Dispose()
            {
                DbConnectionManager.Dispose();
            }
        }
    }
}
