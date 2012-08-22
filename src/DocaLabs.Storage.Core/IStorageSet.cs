using System.Linq;

namespace DocaLabs.Storage.Core
{
    /// <summary>
    /// Represents the collection of that can be queried from the database, of a given type.
    /// </summary>
    /// <typeparam name="TEntity">The type that defines the set.</typeparam>
    public interface IStorageSet<TEntity> : IQueryable<TEntity> 
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Adds an entity to the storage set.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        void Add(TEntity item);

        /// <summary>
        /// Removes an entity from the storage set.
        /// </summary>
        /// <param name="item">The entity to be deleted.</param>
        void Remove(TEntity item);

        /// <summary>
        /// Gets the entity by its primary key value.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>Returns the found entity, or null.</returns>
        TEntity Find(params object[] keyValues);
    }
}