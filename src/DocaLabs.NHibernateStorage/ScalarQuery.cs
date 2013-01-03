using System;
using DocaLabs.Storage.Core.Repositories;
using NHibernate;

namespace DocaLabs.NHibernateStorage
{
    /// <summary>
    /// Implements IQuery interface for NHibermate in order to execute a query on a repository which gives a single value as a result.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    /// <typeparam name="TResult">The result's type.</typeparam>
    public abstract class ScalarQuery<TEntity, TResult> : IScalarQuery<TEntity, TResult>
        where TEntity : class
    {
        /// <summary>
        /// Executes the configured query, delegates to Execute(ISession session) to do actual work.
        /// </summary>
        /// <param name="repository">Repository which provides data.</param>
        /// <returns>A single value result.</returns>
        public TResult Execute(IRepository<TEntity> repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            return Execute(((INHibernateRepositorySession)repository.Session).NHibernateSession);
        }

        /// <summary>
        /// Executes the configured query, the method is called from Execute(IRepository&lt;TEntity> repository) to do the actual work.
        /// </summary>
        /// <param name="session">NHibernate session which provides data.</param>
        /// <returns>A single value result.</returns>
        protected abstract TResult Execute(ISession session);
    }
}