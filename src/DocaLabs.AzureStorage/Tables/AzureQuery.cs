using System;
using System.Collections.Generic;
using DocaLabs.Storage.Core.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace DocaLabs.AzureStorage.Tables
{
    /// <summary>
    /// Implements IQuery interface for CloudTable in order to execute a query on a repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public abstract class AzureQuery<TEntity> : IQuery<TEntity>
        where TEntity : class , ITableEntity
    {
        /// <summary>
        /// Executes the configured query, calls Execute(AzureTableRepositorySession session) to do actual work.
        /// </summary>
        /// <param name="repository">Repository which provides data.</param>
        /// <returns>The list of entities which satisfy the query.</returns>
        public IList<TEntity> Execute(IRepository<TEntity> repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            return Execute(((AzureTableRepositorySession)repository.Session));
        }

        /// <summary>
        /// Executes the configured query, the method is called from Execute(IRepository&lt;TEntity> repository) to do the actual work.
        /// </summary>
        /// <param name="session">AzureTableRepositorySession session which provides access to the CloudTable.</param>
        /// <returns>The list of entities which satisfy the query.</returns>
        protected abstract IList<TEntity> Execute(AzureTableRepositorySession session);
    }
}
