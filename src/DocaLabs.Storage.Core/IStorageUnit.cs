namespace DocaLabs.Storage.Core
{
    /// <summary>
    /// Defines methods for a storage unit of work.
    /// </summary>
    public interface IStorageUnit
    {
        /// <summary>
        /// Saves the changes being tracked to storage.
        /// </summary>
        void SaveChanges();
    }
}
