using System;
using System.Collections.Generic;
using System.Linq;

namespace DocaLabs.Storage.Core.Repositories.DataService
{
    /// <summary>
    /// Implements IQuery interface for Data Service in order to execute a query on a repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public abstract class DataServiceQuery<TEntity> : IQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Executes the configured query, calls Execute(IQueryable&lt;TEntity> query) to do actual work.
        /// </summary>
        /// <param name="repository">Repository which provides data.</param>
        /// <returns>The list of entities which satisfy the query.</returns>
        public IList<TEntity> Execute(IRepository<TEntity> repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            return Execute(((IQueryableRepository<TEntity>)repository).Query);
        }

        /// <summary>
        /// Executes the configured query, the method is called from Execute(IRepository&lt;TEntity> repository) to do the actual work.
        /// </summary>
        /// <param name="query">IQueryable which provides data.</param>
        /// <returns>The list of entities which satisfy the query.</returns>
        protected abstract IList<TEntity> Execute(IQueryable<TEntity> query);
    }
}
