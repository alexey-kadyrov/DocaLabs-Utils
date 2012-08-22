using System.Linq;
using DocaLabs.Storage.Core.DataService;

namespace DocaLabs.Storage.Azure.Tables
{
    /// <summary>
    /// Provides implementation of IDataServiceRepository for Windows Azure table service.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public class AzureTableRepository<TEntity> : DataServiceRepository<TEntity> 
        where TEntity : AzureTableEntity
    {
        /// <summary>
        /// Initializes a new instance of the AzureTableRepository class with the specified context manager.
        /// The table name is resolved from the entity type by EntityTableNameProvider.
        /// </summary>
        /// <param name="contextManager">Context manager.</param>
        public AzureTableRepository(IDataServiceStorageContextManager contextManager) 
            : base(contextManager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AzureTableRepository class with the specified context manager and table name.
        /// </summary>
        /// <param name="contextManager">Context manager.</param>
        /// <param name="tableName">Table name.</param>
        public AzureTableRepository(IDataServiceStorageContextManager contextManager, string tableName) 
            : base(contextManager, tableName)
        {
        }

        /// <summary>
        /// The method is called internally by the Query property to create the service query when necessary.
        /// </summary>
        /// <returns>The new query.</returns>
        protected override IQueryable<TEntity> InitializeQuery()
        {
            return new TableServiceQueryableWrapper<TEntity>(DataService.CreateQuery<TEntity>(TableName));
        }
    }
}
