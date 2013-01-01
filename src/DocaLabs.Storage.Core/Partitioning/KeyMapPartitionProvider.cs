using System;
using System.Collections.Concurrent;
using System.Data;
using System.Transactions;

namespace DocaLabs.Storage.Core.Partitioning
{
    /// <summary>
    /// Contains common methods to get connection from a partition map database.
    /// It uses round bobbin approach by keeping number of keys in each partition equal.
    /// If new partition is added it will be populated until the number of keys won't reach number in others.
    /// </summary>
    public class KeyMapPartitionProvider : IPartitionConnectionProvider
    {
        readonly ConnectionStringMap _connectionStrings;
        readonly ConcurrentDictionary<object, int> _partitionKeyCache;

        /// <summary>
        /// Initializes a new  instance of the KeyMapPartitionProvider class with specified connection to the partition map database.
        /// </summary>
        /// <param name="partitionMapConnectionString">Connection information to a database where to find the partition connection strings.</param>
        public KeyMapPartitionProvider(DatabaseConnectionString partitionMapConnectionString)
        {
            if (partitionMapConnectionString == null)
                throw new ArgumentNullException("partitionMapConnectionString");

            _connectionStrings = new ConnectionStringMap(partitionMapConnectionString);
            _partitionKeyCache = new ConcurrentDictionary<object, int>();
        }

        /// <summary>
        /// Returns a connection for a given partition key. If a partition is not assigned yet for a key
        /// it will be automatically assigned.
        /// </summary>
        /// <param name="partitionKey">The partition key which is used to get associated partition.</param>
        /// <returns>A new instance of a connection wrapper for the partition.</returns>
        /// <exception cref="PartitionException">If the partition's connection is not found.</exception>
        public IDatabaseConnection GetConnection(object partitionKey)
        {
            try
            {
                return new DatabaseConnection(
                    _connectionStrings.GetConnectionString(_partitionKeyCache.GetOrAdd(partitionKey, ExecuteGetOrAutoAssignPartition)));
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
        public void AddNewManualPartition(int partition, DatabaseConnectionString connectionString)
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
        public void AddNewAutoAssignPartition(int partition, DatabaseConnectionString connectionString)
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
            using (var connection = _connectionStrings.PartitionMapConnectionString.CreateDbConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "AddKeyToManualPartition";
                command.CommandType = CommandType.StoredProcedure;

                var p1 = command.CreateParameter();
                p1.ParameterName = "partition";
                p1.Direction = ParameterDirection.Input;
                p1.Value = partition;
                command.Parameters.Add(p1);

                var p2 = command.CreateParameter();
                p2.ParameterName = "key";
                p2.Direction = ParameterDirection.Input;
                p2.Value = partitionKey.ToString();
                command.Parameters.Add(p2);

                connection.Open();

                command.ExecuteNonQuery();

                transaction.Complete();
            }
        }

        int ExecuteGetOrAutoAssignPartition(object partitionKey)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
            using (var connection = _connectionStrings.PartitionMapConnectionString.CreateDbConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "GetOrAutoAssignPartition";
                command.CommandType = CommandType.StoredProcedure;

                var p1 = command.CreateParameter();
                p1.ParameterName = "key";
                p1.Direction = ParameterDirection.Input;
                p1.Value = partitionKey.ToString();
                command.Parameters.Add(p1);

                var r = command.CreateParameter();
                r.ParameterName = "ret_val";
                r.Direction = ParameterDirection.ReturnValue;
                r.DbType = DbType.Int32;
                command.Parameters.Add(r);

                connection.Open();

                command.ExecuteNonQuery();

                transaction.Complete();

                return (int)r.Value;
            }
        }

        void ExecuteAddNewPartition(int partition, DatabaseConnectionString connectionString, bool autoAssign)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
            using (var connection = _connectionStrings.PartitionMapConnectionString.CreateDbConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "AddNewPartition";
                command.CommandType = CommandType.StoredProcedure;

                var p1 = command.CreateParameter();
                p1.ParameterName = "newPartition";
                p1.Direction = ParameterDirection.Input;
                p1.Value = partition;
                command.Parameters.Add(p1);

                var p2 = command.CreateParameter();
                p2.ParameterName = "providerName";
                p2.Direction = ParameterDirection.Input;
                if (connectionString.ProviderName == null)
                    p2.Value = DBNull.Value;
                else
                    p2.Value = connectionString.ProviderName;
                command.Parameters.Add(p2);

                var p3 = command.CreateParameter();
                p3.ParameterName = "connectionString";
                p3.Direction = ParameterDirection.Input;
                p3.Value = connectionString.ConnectionString;
                command.Parameters.Add(p3);

                var p4 = command.CreateParameter();
                p4.ParameterName = "autoAssign";
                p4.Direction = ParameterDirection.Input;
                p4.Value = autoAssign;
                command.Parameters.Add(p4);

                connection.Open();

                command.ExecuteNonQuery();

                transaction.Complete();
            }
        }

        class ConnectionStringMap
        {
            ConcurrentDictionary<int, DatabaseConnectionString> ConnectionStringCache { get; set; }

            public DatabaseConnectionString PartitionMapConnectionString { get; private set; }

            public ConnectionStringMap(DatabaseConnectionString partitionMapConnectionString)
            {
                ConnectionStringCache = new ConcurrentDictionary<int, DatabaseConnectionString>();
                PartitionMapConnectionString = partitionMapConnectionString;
            }

            public DatabaseConnectionString GetConnectionString(int partition)
            {
                return ConnectionStringCache.GetOrAdd(partition, ExecuteGetConnectionString);
            }

            static DatabaseConnectionString MakeConnectionFromCurrentRow(IDataRecord reader)
            {
                return new DatabaseConnectionString(reader.IsDBNull(1) ? null : reader.GetString(1), reader.GetString(2));
            }

            DatabaseConnectionString ExecuteGetConnectionString(int partition)
            {
                using (new TransactionScope(TransactionScopeOption.Suppress))
                using (var connection = PartitionMapConnectionString.CreateDbConnection())
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "GetConnectionString";
                    command.CommandType = CommandType.StoredProcedure;

                    var p1 = command.CreateParameter();
                    p1.ParameterName = "partition";
                    p1.Direction = ParameterDirection.Input;
                    p1.Value = partition;
                    command.Parameters.Add(p1);

                    connection.Open();

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
