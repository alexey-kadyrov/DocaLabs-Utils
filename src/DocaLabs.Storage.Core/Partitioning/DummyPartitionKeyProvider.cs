using System;

namespace DocaLabs.Storage.Core.Partitioning
{
    /// <summary>
    /// Implements IPartitionKeyProvider by always returning empty Guid.
    /// </summary>
    public class DummyPartitionKeyProvider : IPartitionKeyProvider
    {
        /// <summary>
        /// Always returns an empty Guid (Guid.Empty) as the current partition key.
        /// </summary>
        public object GetPartitionKey()
        {
            return Guid.Empty;
        }
    }
}
