using System;
using System.Data.Common;
using DocaLabs.Storage.Core.Utils;

namespace DocaLabs.Storage.Core
{
    /// <summary>
    /// Implements straight wrapper for IDbConnectionWrapper interface.
    /// The wrapped connections is disposed when the wrapper is disposed.
    /// </summary>
    public class DefaultDbConnectionWrapper : IDbConnectionWrapper
    {
        DbConnection _connection;

        DbConnectionString ConnectionString { get; set; }

        /// <summary>
        /// Gets wrapped instance of the DbConnection class.
        /// </summary>
        public DbConnection Connection { get { return _connection ?? (_connection = ConnectionString.CreateDbConnection()); } }

        /// <summary>
        /// Initializes an instance of the DefaultDbConnectionWrapper using provided connection string.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public DefaultDbConnectionWrapper(DbConnectionString connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            ConnectionString = connectionString;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases the resources used by the component.
        /// </summary>
        /// <param name="disposing">true to release resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _connection != null)
                _connection.Dispose();
        }
    }
}