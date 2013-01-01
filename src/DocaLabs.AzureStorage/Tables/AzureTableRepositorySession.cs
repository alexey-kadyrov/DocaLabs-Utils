using System;
using DocaLabs.Storage.Core.Repositories;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace DocaLabs.AzureStorage.Tables
{
    /// <summary>
    /// Manages table operations for AzureTableRepository repository. It uses TableBatchOperation to keep track of changes.
    /// </summary>
    public class AzureTableRepositorySession : IRepositorySession
    {
        readonly TableBatchOperation _batchOperation;

        /// <summary>
        /// Gets wrapped clued table instance.
        /// </summary>
        public CloudTable Table { get; private set; }

        /// <summary>
        /// Gets/sets TableRequestOptions which are used to execute Table batch operations.
        /// </summary>
        public TableRequestOptions RequestOptions { get; set; }

        /// <summary>
        /// Gets/sets OperationContext which is used to execute Table batch operations.
        /// </summary>
        public OperationContext OperationContext { get; set; }

        /// <summary>
        /// Gets/sets the number of storage operations after which the changes will be automatically saved.
        /// The check is done as precondition for Add/Update/Remove methods.
        /// If AutoSaveAfterNumberOperations is zero (which is default value) then no auto save will be done.
        /// </summary>
        public int AutoSaveAfterNumberOperations { get; set; }

        /// <summary>
        /// Initializes a new instance of the class using CloudTable.
        /// </summary>
        public AzureTableRepositorySession(CloudTable table)
        {
            if(table == null)
                throw new ArgumentNullException("table");

            Table = table;
            _batchOperation = new TableBatchOperation();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Saves the changes being tracked to storage.
        /// </summary>
        public void SaveChanges()
        {
            if(_batchOperation.Count == 0)
                return;

            Table.ExecuteBatch(_batchOperation, RequestOptions, OperationContext);

            _batchOperation.Clear();
        }

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        public void Add(ITableEntity entity)
        {
            TryAutoSave();

            _batchOperation.InsertOrReplace(entity);
        }

        /// <summary>
        /// Updates the specified entity in the repository.
        /// </summary>
        public void Update(ITableEntity entity)
        {
            TryAutoSave();

            _batchOperation.Replace(entity);
        }

        /// <summary>
        /// Removes an entity from the storage set.
        /// </summary>
        public void Remove(ITableEntity entity)
        {
            TryAutoSave();

            _batchOperation.Delete(entity);
        }

        /// <summary>
        /// Gets the entity by its partition and row keys.
        /// </summary>
        public TEntity Get<TEntity>(string partitionKey, string rowKey) where TEntity : class , ITableEntity
        {
            return (TEntity)Table.Execute(TableOperation.Retrieve<TEntity>(partitionKey, rowKey)).Result;
        }

        void TryAutoSave()
        {
            if (AutoSaveAfterNumberOperations > 0 && _batchOperation.Count == AutoSaveAfterNumberOperations)
                SaveChanges();
        }
    }
}
