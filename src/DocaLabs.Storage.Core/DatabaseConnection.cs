using System;
using System.Data.Common;

namespace DocaLabs.Storage.Core
{
    /// <summary>
    /// Implements straight wrapper for IDatabaseConnection interface.
    /// The DbConnection is created when the Connection property is accessed for a first time.
    /// The wrapped connections is disposed when the wrapper is disposed.
    /// </summary>
    public class DatabaseConnection : IDatabaseConnection
    {
        DbConnection _connection;
        readonly DatabaseConnectionString _connectionString;

        /// <summary>
        /// Gets wrapped instance of the DbConnection class.
        /// </summary>
        public DbConnection Connection { get { return _connection ?? (_connection = _connectionString.CreateDbConnection()); } }

        /// <summary>
        /// Initializes an instance of the DatabaseConnection using provided connection string.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public DatabaseConnection(DatabaseConnectionString connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            _connectionString = connectionString;
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