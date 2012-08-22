using System;

namespace DocaLabs.Storage.Core
{
    /// <summary>
    /// Defines methods to create an instance of a transient (repository which each time uses a new database connection).
    /// The side effect is that there will be only one repository per connection.
    /// </summary>
    /// <remarks>
    /// It's an abstract class due that interfaces don't allow define nested interfaces.
    /// </remarks>
    public abstract class TransientRepositoryFactory
    {
        /// <summary>
        /// Creates a new instance of a repository using a new database connection each time.
        /// </summary>
        public abstract ITransientRepository<TEntity> Create<TEntity>() where TEntity : class, IEntity;

        /// <summary>
        /// Transient disposable repository.
        /// </summary>
        /// <remarks>
        /// DO NOT implement this interface outside of the factory context because in general a repository doesn't own the context, the context is supplied from outside.
        /// So it would be wrong for any other repository implementations to dispose it's context. 
        /// However in this case it's expected that a factory will create a new repository with a new database connection 
        /// each time when Create is called.
        /// </remarks>
        public interface ITransientRepository<TEntity> : IRepository<TEntity>, IDisposable where TEntity : class, IEntity
        {
        }
    }
}
