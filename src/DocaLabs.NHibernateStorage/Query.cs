using System;
using System.Collections.Generic;
using DocaLabs.Storage.Core.Repositories;
using NHibernate;

namespace DocaLabs.NHibernateStorage
{
    /// <summary>
    /// Implements IQuery interface for NHibermate in order to execute a query on a repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public abstract class Query<TEntity> : IQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Executes the configured query, delegates to Execute(ISession session) to do actual work.
        /// </summary>
        /// <param name="repository">Repository which provides data.</param>
        /// <returns>The list of entities which satisfy the query.</returns>
        public IList<TEntity> Execute(IRepository<TEntity> repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            return Execute(((INHibernateRepositorySession)repository.Session).NHibernateSession);
        }

        /// <summary>
        /// Executes the configured query, the method is called from Execute(IRepository&lt;TEntity> repository) to do the actual work.
        /// </summary>
        /// <param name="session">NHibernate session which provides data.</param>
        /// <returns>The list of entities which satisfy the query.</returns>
        protected abstract IList<TEntity> Execute(ISession session);
    }
}
