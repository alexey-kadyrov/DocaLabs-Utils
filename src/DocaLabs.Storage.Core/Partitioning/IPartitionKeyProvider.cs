namespace DocaLabs.Storage.Core.Partitioning
{
    /// <summary>
    /// Defines methods used to get information about a partition key for a current context.
    /// </summary>
    /// <remarks>
    /// What the current context stands for is defined by your application.
    /// The current key may be current for a thread or for HttpContext.
    /// It must be supplied before any dependant repository is instantiated as it will be used
    /// to get a connection corresponding to the partition key.
    /// For example, for a web applications the partition key can be stored in a session state or in request's Url.
    /// </remarks>
    public interface IPartitionKeyProvider
    {
        /// <summary>
        /// Returns a current partition key for a current context.
        /// What the current context stands for is defined by your application.
        /// </summary>
        object GetPartitionKey();
    }
}
