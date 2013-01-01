using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Manages the IDatabaseConnection using provided connection wrapper.
    /// </summary>
    public class DefaultDbConnectionManager : DbConnectionManagerBase
    {
        /// <summary>
        /// Initializes an instance of the SessionManager class with specified connection wrapper.
        /// </summary>
        /// <param name="connection">Connection wrapper.</param>
        public DefaultDbConnectionManager(IDatabaseConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        /// Opens the context or returns already existing.
        /// </summary>
        /// <returns>The new session object.</returns>
        public override IDatabaseConnection OpenConnection()
        {
            return Connection;
        }
    }
}
