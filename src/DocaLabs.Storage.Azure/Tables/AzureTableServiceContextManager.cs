using System;
using System.Data.Services.Client;
using DocaLabs.Storage.Core.DataService;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace DocaLabs.Storage.Azure.Tables
{
    /// <summary>
    /// Manages the IDataServiceStorageContext provider for use with the Windows Azure Table service.
    /// </summary>
    public class AzureTableServiceContextManager : IDataServiceStorageContextManager
    {
        string BaseAddress { get; set; }
        StorageCredentials Credentials { get; set; }

        /// <summary>
        /// Initializes a new instance of the TableServiceContextManager class which will use AzureStorageFactory to create TableServiceContext.
        /// </summary>
        public AzureTableServiceContextManager()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TableServiceContextManager class with the specified baseAddress and storage credentials.
        /// </summary>
        /// <param name="baseAddress">The Table service endpoint to use create the service context.</param>
        /// <param name="credentials">The account credentials.</param>
        public AzureTableServiceContextManager(string baseAddress, StorageCredentials credentials)
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
                throw new ArgumentNullException("baseAddress");

            if(credentials == null)
                throw new ArgumentNullException("credentials");

            BaseAddress = baseAddress;
            Credentials = credentials;
        }

        /// <summary>
        /// Gets the current IDataServiceStorageContext provider.
        /// </summary>
        public IDataServiceStorageContext DataService { get; private set; }

        /// <summary>
        /// Initializes the IDataServiceStorageContext provider or returns already existing.
        /// </summary>
        /// <returns>The new provider object.</returns>
        public IDataServiceStorageContext InitializeDataService()
        {
            if (!IsInitialized)
            {
                DataService = string.IsNullOrWhiteSpace(BaseAddress) || Credentials == null
                              ? new AzureTableServiceContext(AzureStorageFactory.GetTableServiceContext())
                              : new AzureTableServiceContext(new TableServiceContext(BaseAddress, Credentials)
                              {
                                  IgnoreResourceNotFoundException = true,
                                  SaveChangesDefaultOptions = SaveChangesOptions.ReplaceOnUpdate
                              });
            }

            return DataService;
        }

        /// <summary>
        /// Gets a value indicating whether the IDataServiceStorageContext provider is initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return DataService != null; }
        }
    }
}
