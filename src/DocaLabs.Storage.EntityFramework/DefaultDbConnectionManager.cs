using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Manages the IDbConnectionWrapper using provided connection wrapper.
    /// </summary>
    public class DefaultDbConnectionManager : DbConnectionManagerBase
    {
        /// <summary>
        /// Initializes an instance of the SessionManager class with specified connection wrapper.
        /// </summary>
        /// <param name="connection">Connection wrapper.</param>
        public DefaultDbConnectionManager(IDbConnectionWrapper connection)
            : base(connection)
        {
        }

        /// <summary>
        /// Opens the context or returns already existing.
        /// </summary>
        /// <returns>The new session object.</returns>
        public override IDbConnectionWrapper OpenConnection()
        {
            return Connection;
        }
    }
}
