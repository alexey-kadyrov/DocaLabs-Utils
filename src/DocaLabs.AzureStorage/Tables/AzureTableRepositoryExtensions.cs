using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace DocaLabs.AzureStorage.Tables
{
    /// <summary>
    /// Defines extensions methods specific for Azure table repositories.
    /// </summary>
    public static class AzureTableRepositoryExtensions
    {
        /// <summary>
        /// Updates the list of specified entities in repository.
        /// </summary>
        /// <param name="repository">Target repository.</param>
        /// <param name="items">The entities to update.</param>
        public static void UpdateRange<TEntity>(this IAzureTableRepository<TEntity> repository, IEnumerable<TEntity> items)
            where TEntity : class, ITableEntity
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            if (items == null)
                throw new ArgumentNullException("items");

            foreach (var item in items)
                repository.Update(item);
        }

        /// <summary>
        /// Updates the list of specified entities in repository.
        /// </summary>
        /// <param name="repository">Target repository.</param>
        /// <param name="items">The entities to update.</param>
        public static void UpdateRange<TEntity>(this IAzureTableRepository<TEntity> repository, params TEntity[] items)
            where TEntity : class, ITableEntity
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            repository.UpdateRange((IEnumerable<TEntity>)items);
        }
    }
}
