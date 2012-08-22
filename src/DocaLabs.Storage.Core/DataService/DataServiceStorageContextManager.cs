using System;
using System.Data.Services.Client;

namespace DocaLabs.Storage.Core.DataService
{
    /// <summary>
    /// Manages the IDataServiceStorageContext provider for use with DataServiceContext.
    /// </summary>
    public class DataServiceStorageContextManager : IDataServiceStorageContextManager
    {
        Uri ServiceRoot { get; set; }

        /// <summary>
        /// Initializes a new instance of the DataServiceStorageContextManager class with the specified serviceRoot.
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        public DataServiceStorageContextManager(Uri serviceRoot)
        {
            if(serviceRoot==null)
                throw new ArgumentNullException("serviceRoot");

            ServiceRoot = serviceRoot;
        }

        /// <summary>
        /// Gets the current instance of the IDataServiceStorageContext.
        /// </summary>
        public IDataServiceStorageContext DataService { get; private set; }

        /// <summary>
        /// Initializes a new instance of the IDataServiceStorageContext or returns already existing.
        /// </summary>
        /// <returns>The new provider object.</returns>
        public IDataServiceStorageContext InitializeDataService()
        {
            if (!IsInitialized)
            {
                DataService = new DataServiceStorageContext(new DataServiceContext(ServiceRoot)
                {
                    IgnoreResourceNotFoundException = true,
                    SaveChangesDefaultOptions = SaveChangesOptions.ReplaceOnUpdate
                });
            }

            return DataService;
        }

        /// <summary>
        /// Gets a value indicating whether an instance of the IDataServiceStorageContext is initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return DataService != null; }
        }
    }
}
