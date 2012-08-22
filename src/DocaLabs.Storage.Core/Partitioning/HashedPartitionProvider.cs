using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using DocaLabs.Storage.Core.Utils;

namespace DocaLabs.Storage.Core.Partitioning
{
    /// <summary>
    /// Implementation of IPartitionConnectionProvider which uses partition key hash to get the partition.
    /// The actual connection strings are stored in a database specified by partitionMapConnectionString.
    /// </summary>
    /// <remarks>
    /// Good article explaining hashing issues: http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx
    /// </remarks>
    public class HashedPartitionProvider : IPartitionConnectionProvider
    {
        Dictionary<int, DbConnectionString> ConnectionStringCache { get; set; }

        /// <summary>
        /// Gets number of allocated partitions.
        /// IMPORTANT NOTE: The number of partitions must never change for a given application as this implementation is not rebalancing. 
        /// </summary>
        public int PartitionCount { get { return ConnectionStringCache.Count; } }

        /// <summary>
        /// Initialize an instance of the HashedPartitionProvider class with specified number of partitions.
        /// IMPORTANT NOTE: The number of partitions must never change for a given application as this implementation is not rebalancing. 
        /// </summary>
        /// <param name="partitionMapConnectionString">Connection information to a database where to find the partition connection strings.</param>
        public HashedPartitionProvider(DbConnectionString partitionMapConnectionString)
        {
            if (partitionMapConnectionString == null)
                throw new ArgumentNullException("partitionMapConnectionString");

            try
            {
                CacheAllConnectionStrings(partitionMapConnectionString);
            }
            catch (Exception e)
            {
                throw new PartitionException(Resources.Text.internal_error_see_inner_exception, e);
            }
        }

        void CacheAllConnectionStrings(DbConnectionString partitionMapConnectionString)
        {
            ConnectionStringCache = new Dictionary<int, DbConnectionString>();

            using (new TransactionScope(TransactionScopeOption.Suppress))
            using (var connection = partitionMapConnectionString.CreateDbConnection())
            using (var command = connection.OpenCommand("GetApplicationConnectionStrings"))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                    ConnectionStringCache[PartitionCount] = MakeConnectionFromCurrentRow(reader);
            }
        }

        /// <summary>
        /// Returns a connection for a given partition key.
        /// </summary>
        /// <param name="partitionKey">The partition key which is used to get associated partition.</param>
        /// <returns>A new instance of a connection wrapper for the partition.</returns>
        /// <exception cref="PartitionException">If the partition's connection is not found.</exception>
        public IDbConnectionWrapper GetConnection(object partitionKey)
        {
            return new DefaultDbConnectionWrapper(ConnectionStringCache[CalculatePartition(partitionKey)]);
        }

        /// <summary>
        /// Calculates the partition number based on the key hash and number of available partitions.
        /// </summary>
        /// <remarks>
        /// The documentation on GetHashCode doesn't mention it to be unstable (unlike as for string.GetHashCode).
        /// </remarks>
        /// <param name="partitionKey">Key for which calculate the partition number.</param>
        /// <returns>Partition number.</returns>
        public int CalculatePartition(object partitionKey)
        {
            // A hash code can be negative, and thus its remainder can be negative also.
            // Do the math in unsigned ints to be sure we stay positive.
            return (int)(((uint)partitionKey.GetHashCode()) % (uint)PartitionCount);
        }

        static DbConnectionString MakeConnectionFromCurrentRow(IDataRecord reader)
        {
            return new DbConnectionString(reader.IsDBNull(1) ? null : reader.GetString(1), reader.GetString(2));
        }
    }
}
