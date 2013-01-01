using System;
using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Defines methods to manage IDatabaseConnection instances.
    /// </summary>
    public interface IDbConnectionManager : IDisposable
    {
        /// <summary>
        /// Gets the current context.
        /// </summary>
        IDatabaseConnection Connection { get; }

        /// <summary>
        /// Opens the context or returns already existing.
        /// </summary>
        /// <returns>The new session object.</returns>
        IDatabaseConnection OpenConnection();

        /// <summary>
        /// Gets a value indicating whether the context is open.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Creates a new instance of the repository context.
        /// </summary>
        IRepositoryContext<TEntity> CreateRepositoryContext<TEntity>() where TEntity : class, IEntity;
    }
}
