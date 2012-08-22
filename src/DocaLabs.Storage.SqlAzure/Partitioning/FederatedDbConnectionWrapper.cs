using System.Data;
using System.Data.Common;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Utils;

namespace DocaLabs.Storage.SqlAzure.Partitioning
{
    /// <summary>
    /// Implements IDbConnectionWrapper to be used with Sql Azure federations.
    /// </summary>
    public class FederatedDbConnectionWrapper : IDbConnectionWrapper
    {
        DbConnection _connection;

        DbConnectionString ConnectionString { get; set; }

        /// <summary>
        /// An initialized federation command to be executed when the connection is opened.
        /// </summary>
        protected FederationCommand FederationCommand { get; private set; }

        /// <summary>
        /// Gets wrapped instance of the DbConnection class.
        /// </summary>
        public DbConnection Connection
        {
            get
            {
                if(_connection == null)
                {
                    _connection = ConnectionString.CreateDbConnection();
                    _connection.StateChange += ConnectionOnStateChange;
                }

                return _connection;
            }
        }

        /// <summary>
        /// Initializes an instance of the FederatedDbConnectionWrapper using the provided connection string and the statement that should be executed when connection is opened.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="federationCommand">Command that will be executed when the connection is opened.</param>
        public FederatedDbConnectionWrapper(DbConnectionString connectionString, FederationCommand federationCommand)
        {
            ConnectionString = connectionString;
            FederationCommand = federationCommand;
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
            if ((!disposing) || _connection == null) 
                return;

            _connection.StateChange -= ConnectionOnStateChange;
            _connection.Dispose();
        }

        /// <summary>
        /// Catches the ConnectionOnStateChange event in order to detect when the connection is open in order to execute the federation statement.
        /// </summary>
        protected virtual void ConnectionOnStateChange(object sender, StateChangeEventArgs stateChangeEventArgs)
        {
            if (stateChangeEventArgs.CurrentState != ConnectionState.Open)
                return;

            ExecuteStatement();
        }

        /// <summary>
        /// Executes the federation command when the connection is opened.
        /// </summary>
        protected virtual void ExecuteStatement()
        {
            FederationCommand.Execute(_connection);
        }
    }
}
