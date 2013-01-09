using System;
using System.Collections.Generic;
using System.Data.Entity;
using DocaLabs.Storage.Core.Repositories;

namespace DocaLabs.EntityFrameworkStorage
{
    /// <summary>
    /// Implements IQuery interface for Entity Framework in order to execute a query on a repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public abstract class Query<TEntity> : IQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Executes the configured query, delegates to Execute(DbContext context) to do actual work.
        /// </summary>
        /// <param name="repository">Repository which provides data.</param>
        /// <returns>The list of entities which satisfy the query.</returns>
        public IList<TEntity> Execute(IRepository<TEntity> repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            return Execute(((IDbRepositorySession)repository.Session).Context);
        }

        /// <summary>
        /// Executes the configured query, the method is called from Execute(IRepository&lt;TEntity> repository) to do the actual work.
        /// </summary>
        /// <param name="context">DbContext which provides data.</param>
        /// <returns>The list of entities which satisfy the query.</returns>
        protected abstract IList<TEntity> Execute(DbContext context);
    }
}
