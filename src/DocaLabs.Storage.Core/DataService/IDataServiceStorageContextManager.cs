namespace DocaLabs.Storage.Core.DataService
{
    /// <summary>
    /// Defines methods to manage an instance of the IDataServiceStorageContext.
    /// </summary>
    public interface IDataServiceStorageContextManager
    {
        /// <summary>
        /// Gets the current instance of the IDataServiceStorageContext.
        /// </summary>
        IDataServiceStorageContext DataService { get; }

        /// <summary>
        /// Initializes a new instance of the IDataServiceStorageContext or returns already existing.
        /// </summary>
        IDataServiceStorageContext InitializeDataService();

        /// <summary>
        /// Gets a value indicating whether an instance of the IDataServiceStorageContext is initialized.
        /// </summary>
        bool IsInitialized { get; }
    }
}
