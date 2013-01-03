using System;
using DocaLabs.Storage.Core.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace DocaLabs.AzureStorage.Tables
{
    /// <summary>
    /// Implements IQuery interface for CloudTable in order to execute a query on a repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    /// <typeparam name="TResult">The result's type.</typeparam>
    public abstract class AzureScalarQuery<TEntity, TResult> : IScalarQuery<TEntity, TResult>
        where TEntity : class , ITableEntity
    {
        /// <summary>
        /// Executes the configured query, calls Execute(AzureTableRepositorySession session) to do actual work.
        /// </summary>
        /// <param name="repository">Repository which provides data.</param>
        /// <returns>A single value result.</returns>
        public TResult Execute(IRepository<TEntity> repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            return Execute(((AzureTableRepositorySession)repository.Session));
        }

        /// <summary>
        /// Executes the configured query, the method is called from Execute(IRepository&lt;TEntity> repository) to do the actual work.
        /// </summary>
        /// <param name="session">AzureTableRepositorySession session which provides access to the CloudTable.</param>
        /// <returns>A single value result.</returns>
        protected abstract TResult Execute(AzureTableRepositorySession session);
    }
}