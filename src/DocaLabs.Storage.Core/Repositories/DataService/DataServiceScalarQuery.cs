using System;
using System.Linq;

namespace DocaLabs.Storage.Core.Repositories.DataService
{
    /// <summary>
    /// Implements IQuery interface for Data Service in order to execute a query on a repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    /// <typeparam name="TResult">The result's type.</typeparam>
    public abstract class DataServiceScalarQuery<TEntity, TResult> : IScalarQuery<TEntity, TResult>
        where TEntity : class
    {
        /// <summary>
        /// Executes the configured query, calls Execute(IQueryable&lt;TEntity> query) to do actual work.
        /// </summary>
        /// <param name="repository">Repository which provides data.</param>
        /// <returns>A single value result.</returns>
        public TResult Execute(IRepository<TEntity> repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            return Execute(((IQueryableRepository<TEntity>)repository).Query);
        }

        /// <summary>
        /// Executes the configured query, the method is called from Execute(IRepository&lt;TEntity> repository) to do the actual work.
        /// </summary>
        /// <param name="query">IQueryable which provides data.</param>
        /// <returns>A single value result.</returns>
        protected abstract TResult Execute(IQueryable<TEntity> query);
    }
}