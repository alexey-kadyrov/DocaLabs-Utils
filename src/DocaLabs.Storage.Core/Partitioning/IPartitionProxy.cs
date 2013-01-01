namespace DocaLabs.Storage.Core.Partitioning
{
    /// <summary>
    /// Defines methods to get connection associated with a current partition key.
    /// </summary>
    public interface IPartitionProxy
    {
        /// <summary>
        /// Returns a new connection instance for the current partition key.
        /// </summary>
        IDatabaseConnection GetConnection();
    }
}
