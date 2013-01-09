using System;
using System.Data.Entity;
using DocaLabs.Storage.Core.Repositories;

namespace DocaLabs.EntityFrameworkStorage
{
    /// <summary>
    /// Implements IQuery interface for Entity Framework in order to execute a query on a repository which gives a single value as a result.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    /// <typeparam name="TResult">The result's type.</typeparam>
    public abstract class ScalarQuery<TEntity, TResult> : IScalarQuery<TEntity, TResult>
        where TEntity : class 
    {
        /// <summary>
        /// Executes the configured query, delegates to Execute(DbContext context) to do actual work.
        /// </summary>
        /// <param name="repository">Repository which provides data.</param>
        /// <returns>A single value result.</returns>
        public TResult Execute(IRepository<TEntity> repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            return Execute(((IDbRepositorySession)repository.Session).Context);
        }

        /// <summary>
        /// Executes the configured query, the method is called from Execute(IRepository&lt;TEntity> repository) to do the actual work.
        /// </summary>
        /// <param name="context">DbContext which provides data.</param>
        /// <returns>A single value result.</returns>
        protected abstract TResult Execute(DbContext context);
    }
}
