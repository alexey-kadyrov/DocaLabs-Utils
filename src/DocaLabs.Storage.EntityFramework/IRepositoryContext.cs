using System;
using System.Data.Entity;
using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Defines methods for querying and working with entity data as objects.
    /// </summary>
    public interface IRepositoryContext<TEntity> : IStorageContext, IDisposable
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Gets the root context which manages the entity.
        /// </summary>
        DbContextAggregateRoot<TEntity> Context { get; }

        /// <summary>
        /// Rereads the state of the given instance from the underlying database
        /// </summary>
        void Refresh(object obj);

        /// <summary>
        /// Returns a DbSet for the specified type.
        /// </summary>
        /// <returns>A DbSet instance for the given entity type.</returns>
        IDbSet<TEntity> GetSet();
    }
}
