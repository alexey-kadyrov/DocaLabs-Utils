using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;

namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Manages the IDatabaseConnection using CurrentPartitionProxy to get the connection string.
    /// </summary>
    public class PartitionedDbConnectionManager : DbConnectionManagerBase
    {
        /// <summary>
        /// Opens the context or returns already existing.
        /// </summary>
        /// <returns>The new session object.</returns>
        public override IDatabaseConnection OpenConnection()
        {
            if (!IsOpen)
                Connection = CurrentPartitionProxy.Current.GetConnection();

            return Connection;
        }
    }
}
