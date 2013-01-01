using System;
using System.Configuration;
using System.Data.Common;

namespace DocaLabs.Storage.Core
{
    /// <summary>
    /// Represents information about a storage connection string.
    /// </summary>
    public class DatabaseConnectionString
    {
        /// <summary>
        /// Gets the storage provider name.
        /// </summary>
        /// 
        public string ProviderName { get; private set; }

        /// <summary>
        /// Gets the storage connection string.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Initializes a new instance of the DatabaseConnectionString class using specified connection string and System.Data.SqlClient as default provider.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public DatabaseConnectionString(string connectionString)
            : this(null, connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DatabaseConnectionString class using specified provider and connection string.
        /// </summary>
        /// <param name="providerName">Provider name.</param>
        /// <param name="connectionString">Connection string.</param>
        public DatabaseConnectionString(string providerName, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("connectionString");

            ProviderName = providerName;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Initializes a new instance of the DatabaseConnectionString class using information specified by ConnectionStringSettings instance. 
        /// </summary>
        /// <param name="connectionSettings">ConnectionStringSettings.</param>
        public DatabaseConnectionString(ConnectionStringSettings connectionSettings)
        {
            if (connectionSettings == null)
                throw new ArgumentNullException("connectionSettings");

            if (string.IsNullOrWhiteSpace(connectionSettings.ConnectionString))
                throw new ArgumentNullException("connectionSettings");

            ProviderName = connectionSettings.ProviderName;
            ConnectionString = connectionSettings.ConnectionString;
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the DbConnection class using ProviderName and the ConnectionString.
        /// </summary>
        /// <remarks>
        /// If the ProviderName is null or white space string then the System.Data.SqlClient is used.
        /// </remarks>
        /// <returns>A new instance of DbConnection.</returns>
        public virtual DbConnection CreateDbConnection()
        {
            var provider= DbProviderFactories.GetFactory(string.IsNullOrWhiteSpace(ProviderName) 
                ? "System.Data.SqlClient" 
                : ProviderName);

            var connection = provider.CreateConnection();
            if(connection != null)
                connection.ConnectionString = ConnectionString;

            return connection;
        }
    }
}
