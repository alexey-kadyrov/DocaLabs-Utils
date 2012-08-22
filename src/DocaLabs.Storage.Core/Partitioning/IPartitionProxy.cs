namespace DocaLabs.Storage.Core.Partitioning
{
    /// <summary>
    /// Defines methods to get connection associated with a current partition key.
    /// The interface acts as a facade for IPartitionKeyProvider and IPartitionConnectionProvider.
    /// </summary>
    public interface IPartitionProxy
    {
        /// <summary>
        /// Gets implementation of the provider which supplies a current partition key.
        /// </summary>
        IPartitionKeyProvider PartitionKeyProvider { get; }

        /// <summary>
        /// Gets implementation of a provider which supplies connection for a given partition key.
        /// </summary>
        IPartitionConnectionProvider PartitionConnectionProvider { get; }

        /// <summary>
        /// Returns a new connection instance for the current partition key.
        /// </summary>
        /// <remarks>
        /// The current partition key is supplied by implementation of the IPartitionKeyProvider interface.
        /// </remarks>
        IDbConnectionWrapper GetConnection();
    }
}
