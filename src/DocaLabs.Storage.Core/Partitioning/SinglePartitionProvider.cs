using System;
using DocaLabs.Storage.Core.Utils;

namespace DocaLabs.Storage.Core.Partitioning
{
    /// <summary>
    /// Implementation of IPartitionConnectionProvider interface which always returns connection for configured connection string.
    /// </summary>
    public class SinglePartitionProvider : IPartitionConnectionProvider
    {
        /// <summary>
        /// Gets the current connection string.
        /// </summary>
        public DbConnectionString ConnectionString { get; private set; }

        /// <summary>
        /// Initializes a new instance of the SinglePartitionProvider using specified connection string instance.
        /// </summary>
        /// <param name="connectionString">Instance of the DbConnectionString class.</param>
        public SinglePartitionProvider(DbConnectionString connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            ConnectionString = connectionString;
        }

        /// <summary>
        /// Always returns the configured connection for the same connection string regardless of the partition key argument.
        /// </summary>
        /// <returns>A new instance of a connection wrapper, the partition parameter is ignored.</returns>
        public IDbConnectionWrapper GetConnection(object partitionKey)
        {
            return new DefaultDbConnectionWrapper(ConnectionString);
        }
    }
}
