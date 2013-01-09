using System.Data.Entity;
using DocaLabs.Storage.Core.Repositories;

namespace DocaLabs.EntityFrameworkStorage
{
    /// <summary>
    /// Defines methods to manage DbContext sessions.
    /// </summary>
    public interface IDbRepositorySession : IRepositoryFactory
    {
        /// <summary>
        /// OCreates the context or returns already existing.
        /// </summary>
        /// <returns>The session object.</returns>
        DbContext Context { get; }

        /// <summary>
        /// Returns a IDbSet for the specified entity type.
        /// </summary>
        IDbSet<TEntity> GetSet<TEntity>() where TEntity : class;
    }
}