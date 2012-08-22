namespace DocaLabs.Storage.Core.Partitioning
{
    /// <summary>
    /// Defines methods that are used to get partition' connection string.
    /// </summary>
    public interface IPartitionConnectionProvider
    {
        /// <summary>
        /// Returns a connection for a given partition key.
        /// </summary>
        /// <param name="partitionKey">The partition key which is used to get associated partition.</param>
        /// <returns>A new instance of a connection wrapper for the partition.</returns>
        /// <exception cref="PartitionException">If the partition's connection is not found.</exception>
        IDbConnectionWrapper GetConnection(object partitionKey);
    }
}
