using System;

namespace DocaLabs.Storage.Core.Partitioning
{
    /// <summary>
    /// Implements IPartitionProxy interface to get connection associated with a current partition key.
    /// The interface acts as a facade for IPartitionKeyProvider and IPartitionConnectionProvider.
    /// </summary>
    public class PartitionProxy : IPartitionProxy
    {
        /// <summary>
        /// Initializes an instance of the PartitionProxy class with specified IPartitionKeyProvider and IPartitionConnectionProvider implantations.
        /// </summary>
        /// <param name="partitionKeyProvider">Current partition key provider.</param>
        /// <param name="partitionProvider">Partition provider.</param>
        public PartitionProxy(IPartitionKeyProvider partitionKeyProvider, IPartitionConnectionProvider partitionProvider)
        {
            if(partitionKeyProvider == null)
                throw new ArgumentNullException("partitionKeyProvider");

            if(partitionProvider == null)
                throw new ArgumentNullException("partitionProvider");

            PartitionKeyProvider = partitionKeyProvider;
            PartitionConnectionProvider = partitionProvider;
        }

        /// <summary>
        /// Gets/sets implementation to get a current partition key.
        /// </summary>
        public IPartitionKeyProvider PartitionKeyProvider { get; private set; }

        /// <summary>
        /// Gets/sets implementation to get connection for a given partition key.
        /// </summary>
        public IPartitionConnectionProvider PartitionConnectionProvider { get; private set; }

        /// <summary>
        /// Returns a new instance of a connection wrapper for the current partition key.
        /// </summary>
        /// <remarks>
        /// The current partition key is supplied by implementation of the IPartitionKeyProvider interface.
        /// </remarks>
        public IDatabaseConnection GetConnection()
        {
            return PartitionConnectionProvider.GetConnection(PartitionKeyProvider.GetPartitionKey());
        }
    }
}
