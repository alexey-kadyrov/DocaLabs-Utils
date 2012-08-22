using System;
using System.Data.Services.Common;
using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.Azure.Tables
{
    /// <summary>
    /// Represents an entity in a Windows Azure table.
    /// </summary>
    [DataServiceKey("PartitionKey", "RowKey")]
    public abstract class AzureTableEntity : IEntity
    {
        /// <summary>
        /// Gets or sets the partition key of a table entity.
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// Gets or sets the row key of a table entity.
        /// </summary>
        public string RowKey { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for the entity.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Initializes a new instance of the AzureTableEntity class.
        /// </summary>
        protected AzureTableEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AzureTableEntity class.
        /// </summary>
        /// <param name="partitionKey">The partition key of a table entity.</param>
        /// <param name="rowKey">The row key of a table entity.</param>
        protected AzureTableEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        /// <summary>
        /// Initializes a new instance of the AzureTableEntity class.
        /// </summary>
        /// <param name="partitionKey">The partition key of a table entity.</param>
        /// <param name="rowKey">The row key of a table entity.</param>
        protected AzureTableEntity(Guid partitionKey, Guid rowKey)
            : this(partitionKey.ToString(), rowKey.ToString())
        {
        }
    }
}
