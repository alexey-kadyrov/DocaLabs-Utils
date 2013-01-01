using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;

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
        Dictionary<int, DatabaseConnectionString> _connectionStringCache;

        /// <summary>
        /// Gets number of allocated partitions.
        /// IMPORTANT NOTE: The number of partitions must never change for a given application as this implementation is not rebalancing. 
        /// </summary>
        public int PartitionCount { get { return _connectionStringCache.Count; } }

        /// <summary>
        /// Initialize an instance of the HashedPartitionProvider class with specified number of partitions.
        /// IMPORTANT NOTE: The number of partitions must never change for a given application as this implementation is not rebalancing. 
        /// </summary>
        /// <param name="partitionMapConnectionString">Connection information to a database where to find the partition connection strings.</param>
        public HashedPartitionProvider(DatabaseConnectionString partitionMapConnectionString)
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

        void CacheAllConnectionStrings(DatabaseConnectionString partitionMapConnectionString)
        {
            _connectionStringCache = new Dictionary<int, DatabaseConnectionString>();

            using (new TransactionScope(TransactionScopeOption.Suppress))
            using (var connection = partitionMapConnectionString.CreateDbConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "GetApplicationConnectionStrings";
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        _connectionStringCache[PartitionCount] = MakeConnectionFromCurrentRow(reader);
                }
            }
        }

        /// <summary>
        /// Returns a connection for a given partition key.
        /// </summary>
        /// <param name="partitionKey">The partition key which is used to get associated partition.</param>
        /// <returns>A new instance of a connection wrapper for the partition.</returns>
        /// <exception cref="PartitionException">If the partition's connection is not found.</exception>
        public IDatabaseConnection GetConnection(object partitionKey)
        {
            return new DatabaseConnection(_connectionStringCache[CalculatePartition(partitionKey)]);
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

        static DatabaseConnectionString MakeConnectionFromCurrentRow(IDataRecord reader)
        {
            return new DatabaseConnectionString(reader.IsDBNull(1) ? null : reader.GetString(1), reader.GetString(2));
        }
    }
}
