using System;
using DocaLabs.Storage.Core;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace DocaLabs.AzureStorage.Tables
{
    /// <summary>
    /// Provides utility methods for Windows Azure table service.
    /// </summary>
    public static class TableExtensions
    {
        /// <summary>
        /// Creates a table if it doesn't exist on the default endpoints for the CloudStorageAccount using explicit table names.
        /// </summary>
        /// <param name="account">Cloud storage account.</param>
        /// <param name="names">Table names.</param>
        public static void CreateTableIfNotExist(this CloudStorageAccount account, params string[] names)
        {
            account.CreateCloudTableClient().CreateTableIfNotExist(names);
        }

        /// <summary>
        /// Creates a table if it doesn't exist for the CloudTableClient using explicit table names.
        /// </summary>
        /// <param name="tableClient">Cloud table client.</param>
        /// <param name="names">Table names.</param>
        public static void CreateTableIfNotExist(this CloudTableClient tableClient, params string[] names)
        {
            foreach (var name in names)
                tableClient.GetTableReference(name).CreateIfNotExists();
        }

        /// <summary>
        /// Creates a table if it doesn't exist on the default endpoints for the CloudStorageAccount by resolving entity types to table names.
        /// </summary>
        /// <param name="account">Cloud storage account.</param>
        /// <param name="entities">Entity types which should be resolved into table names.</param>
        public static void CreateTableIfNotExist(this CloudStorageAccount account, params Type[] entities)
        {
            account.CreateCloudTableClient().CreateTableIfNotExist(entities);
        }


        /// <summary>
        /// Creates a table if it doesn't exist for the CloudTableClient by resolving entity types to table names.
        /// </summary>
        /// <param name="tableClient">Cloud table client.</param>
        /// <param name="entities">Entity types which should be resolved into table names.</param>
        public static void CreateTableIfNotExist(this CloudTableClient tableClient, params Type[] entities)
        {
            foreach (var entity in entities)
                tableClient.GetTableReference(EntityToNameMap.Get(entity)).CreateIfNotExists();
        }
    }
}
