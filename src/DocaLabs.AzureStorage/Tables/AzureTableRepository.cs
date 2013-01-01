using System;
using System.Collections.Generic;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace DocaLabs.AzureStorage.Tables
{
    /// <summary>
    /// Provides implementation of IDataServiceRepository for Windows Azure table service.
    /// The repository is uses TableBatchOperation to keep track of changes.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public class AzureTableRepository<TEntity> : IAzureTableRepository<TEntity>
        where TEntity : class, ITableEntity
    {
        readonly AzureTableRepositorySession _session;

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Provides access to unit of work for such operations as committing any pending changes.
        /// </summary>
        public IRepositorySession Session { get { return _session; } }

        /// <summary>
        /// Initializes a new instance of the AzureTableRepository class using cloud table client and a table name.
        /// </summary>
        /// <param name="tableClient">Cloud table client.</param>
        /// <param name="tableName">Table name, if the value is null or white space string then the table name is resolved using EntityToNameMap.</param>
        public AzureTableRepository(CloudTableClient tableClient, string tableName = null)
        {
            if (tableClient == null)
                throw new ArgumentNullException("tableClient");

            TableName = string.IsNullOrWhiteSpace(tableName)
                ? EntityToNameMap.Get<TEntity>()
                : tableName;

            _session = new AzureTableRepositorySession(tableClient.GetTableReference(TableName));
        }

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(TEntity entity)
        {
            _session.Add(entity);
        }

        /// <summary>
        /// Removes an entity from the storage set.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        public void Remove(TEntity entity)
        {
            _session.Remove(entity);
        }

        /// <summary>
        /// Gets the entity by its primary key value.
        /// </summary>
        /// <param name="keyValues">
        /// The values of the primary key for the entity to be found.
        /// There must be two non null values: first is for the PartitionKey and the second is for the RowKey.
        /// </param>
        /// <returns>Returns the found entity, or null.</returns>
        public TEntity Get(params object[] keyValues)
        {
            if(keyValues == null)
                throw new ArgumentNullException("keyValues");

            if(keyValues.Length != 2)
                throw new ArgumentException(Resources.Text.expected_two_partition_and_row_keys, "keyValues");

            if(keyValues[0] == null || keyValues[1] == null)
                throw new ArgumentException(Resources.Text.expected_two_non_null_partition_and_row_keys, "keyValues");

            return _session.Get<TEntity>(keyValues[0].ToString(), keyValues[1].ToString());
        }

        /// <summary>
        /// Executes the configured query.
        /// </summary>
        /// <returns>The list of entities which satisfy the query.</returns>
        public IList<TEntity> Execute(IQuery<TEntity> query)
        {
            if(query == null)
                throw new ArgumentNullException("query");

            return query.Execute(this);
        }

        /// <summary>
        /// Updates the specified entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Update(TEntity entity)
        {
            _session.Update(entity);
        }
    }
}
