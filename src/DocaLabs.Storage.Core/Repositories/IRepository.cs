using System.Collections.Generic;

namespace DocaLabs.Storage.Core.Repositories
{
    /// <summary>
    /// Defines methods for a persistable repository for entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public interface IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Provides access to unit of work for such operations as committing any pending changes.
        /// </summary>
        IRepositorySession Session { get; }

        /// <summary>
        /// Adds an entity to the storage set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Removes an entity from the storage set.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        void Remove(TEntity entity);

        /// <summary>
        /// Gets the entity by its primary key value.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>Returns the found entity, or null.</returns>
        TEntity Get(params object[] keyValues);

        /// <summary>
        /// Executes the configured query which gives a list of entities as a result.
        /// </summary>
        /// <returns>The list of entities which satisfy the query.</returns>
        IList<TEntity> Execute(IQuery<TEntity> query);

        /// <summary>
        /// Executes the configured query which gives a list of entities as a result.
        /// </summary>
        /// <returns>The result of the query.</returns>
        TResult Execute<TResult>(IScalarQuery<TEntity, TResult> query);
    }
}
