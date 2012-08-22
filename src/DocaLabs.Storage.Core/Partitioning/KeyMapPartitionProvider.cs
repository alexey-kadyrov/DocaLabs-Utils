using System;
using System.Collections.Concurrent;
using System.Data;
using System.Transactions;
using DocaLabs.Storage.Core.Utils;

namespace DocaLabs.Storage.Core.Partitioning
{
    /// <summary>
    /// Contains common methods to get connection from a partition map database.
    /// It uses round bobbin approach by keeping number of keys in each partition equal.
    /// If new partition is added it will be populated until the number of keys won't reach number in others.
    /// </summary>
    public class KeyMapPartitionProvider : IPartitionConnectionProvider
    {
        ConnectionStringMap ConnectionStrings { get; set; }
        ConcurrentDictionary<object, int> PartitionKeyCache { get; set; }

        /// <summary>
        /// Initializes a new  instance of the KeyMapPartitionProvider class with specified connection to the partition map database.
        /// </summary>
        /// <param name="partitionMapConnectionString">Connection information to a database where to find the partition connection strings.</param>
        public KeyMapPartitionProvider(DbConnectionString partitionMapConnectionString)
        {
            if (partitionMapConnectionString == null)
                throw new ArgumentNullException("partitionMapConnectionString");

            ConnectionStrings = new ConnectionStringMap(partitionMapConnectionString);
            PartitionKeyCache = new ConcurrentDictionary<object, int>();
        }

        /// <summary>
        /// Returns a connection for a given partition key. If a partition is not assigned yet for a key
        /// it will be automatically assigned.
        /// </summary>
        /// <param name="partitionKey">The partition key which is used to get associated partition.</param>
        /// <returns>A new instance of a connection wrapper for the partition.</returns>
        /// <exception cref="PartitionException">If the partition's connection is not found.</exception>
        public IDbConnectionWrapper GetConnection(object partitionKey)
        {
            try
            {
                return new DefaultDbConnectionWrapper(
                    ConnectionStrings.GetConnectionString(PartitionKeyCache.GetOrAdd(partitionKey, ExecuteGetOrAutoAssignPartition)));
            }
            catch (PartitionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new PartitionException(string.Format(Resources.Text.failed_get_connection_string_for_partition_key_0, partitionKey), e);
            }
        }

        /// <summary>
        /// Adds a partition which cannot be automatically expanded, keys can be added only explicitly.
        /// </summary>
        /// <param name="partition">Partition to create.</param>
        /// <param name="connectionString">Connection string to be used for the partition.</param>
        public void AddNewManualPartition(int partition, DbConnectionString connectionString)
        {
            if (partition < 0)
                throw new ArgumentOutOfRangeException("partition");

            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            try
            {
                ExecuteAddNewPartition(partition, connectionString, false);
            }
            catch (Exception e)
            {
                throw new PartitionException(Resources.Text.internal_error_see_inner_exception, e);
            }
        }

        /// <summary>
        /// Creates a partition which can grow automatically each time GetConnectionString is called for a new partition key.
        /// </summary>
        /// <param name="partition">Partition to create.</param>
        /// <param name="connectionString">Connection string to be used for the partition.</param>
        public void AddNewAutoAssignPartition(int partition, DbConnectionString connectionString)
        {
            if (partition < 0)
                throw new ArgumentOutOfRangeException("partition");

            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            try
            {
                ExecuteAddNewPartition(partition, connectionString, true);
            }
            catch (Exception e)
            {
                throw new PartitionException(Resources.Text.internal_error_see_inner_exception, e);
            }
        }

        /// <summary>
        /// Explicitly adds a new key to a manual partition.
        /// </summary>
        /// <param name="partition">Partition where to add the key, the partition must be created as the manual.</param>
        /// <param name="partitionKey">New partition key.</param>
        public void AddKeyToManualPartition(int partition, object partitionKey)
        {
            if (partition < 0)
                throw new ArgumentOutOfRangeException("partition");

            try
            {
                ExecuteAddKeyToManualPartition(partition, partitionKey);
            }
            catch (Exception e)
            {
                throw new PartitionException(Resources.Text.internal_error_see_inner_exception, e);
            }
        }

        void ExecuteAddKeyToManualPartition(int partition, object partitionKey)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
            using (var connection = ConnectionStrings.PartitionMapConnectionString.CreateDbConnection())
            using (var command = connection.OpenCommand("AddKeyToManualPartition"))
            {
                command
                    .With("partition", partition)
                    .With("key", partitionKey.ToString());

                command.ExecuteNonQuery();

                transaction.Complete();
            }
        }

        int ExecuteGetOrAutoAssignPartition(object partitionKey)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
            using (var connection = ConnectionStrings.PartitionMapConnectionString.CreateDbConnection())
            using (var command = connection.OpenCommand("GetOrAutoAssignPartition"))
            {
                command
                    .With("key", partitionKey.ToString())
                    .Return();

                var partition = command.Execute();

                transaction.Complete();

                return partition;
            }
        }

        void ExecuteAddNewPartition(int partition, DbConnectionString connectionString, bool autoAssign)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
            using (var connection = ConnectionStrings.PartitionMapConnectionString.CreateDbConnection())
            using (var command = connection.OpenCommand("AddNewPartition"))
            {
                command
                    .With("newPartition", partition)
                    .With("providerName", connectionString.ProviderName)
                    .With("connectionString", connectionString.ConnectionString)
                    .With("autoAssign", autoAssign);

                command.ExecuteNonQuery();

                transaction.Complete();
            }
        }

        class ConnectionStringMap
        {
            ConcurrentDictionary<int, DbConnectionString> ConnectionStringCache { get; set; }

            public DbConnectionString PartitionMapConnectionString { get; private set; }

            public ConnectionStringMap(DbConnectionString partitionMapConnectionString)
            {
                ConnectionStringCache = new ConcurrentDictionary<int, DbConnectionString>();
                PartitionMapConnectionString = partitionMapConnectionString;
            }

            public DbConnectionString GetConnectionString(int partition)
            {
                return ConnectionStringCache.GetOrAdd(partition, ExecuteGetConnectionString);
            }

            static DbConnectionString MakeConnectionFromCurrentRow(IDataRecord reader)
            {
                return new DbConnectionString(reader.IsDBNull(1) ? null : reader.GetString(1), reader.GetString(2));
            }

            DbConnectionString ExecuteGetConnectionString(int partition)
            {
                using (new TransactionScope(TransactionScopeOption.Suppress))
                using (var connection = PartitionMapConnectionString.CreateDbConnection())
                using (var command = connection.OpenCommand("GetConnectionString"))
                {
                    command.With("partition", partition);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return MakeConnectionFromCurrentRow(reader);

                        throw new PartitionException(
                            string.Format(Resources.Text.failed_get_connection_string_for_partition_0, partition));
                    }
                }
            }
        }
    }
}
