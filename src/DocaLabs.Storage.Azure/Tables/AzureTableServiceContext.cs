using DocaLabs.Storage.Core.DataService;
using Microsoft.WindowsAzure.StorageClient;

namespace DocaLabs.Storage.Azure.Tables
{
    /// <summary>
    /// The wrapper around runtime context of the table service.
    /// </summary>
    public class AzureTableServiceContext : DataServiceStorageContext
    {
        /// <summary>
        /// Initializes the instance of the AzureTableServiceContext.
        /// </summary>
        /// <param name="context">The runtime context of the table service.</param>
        public AzureTableServiceContext(TableServiceContext context) 
            : base(context)
        {
        }

        /// <summary>
        /// Saves the changes being tracked to storage.
        /// </summary>
        public override void SaveChanges()
        {
            ((TableServiceContext)Context).SaveChangesWithRetries(SaveChangesDefaultOptions);
        }

        /// <summary>
        /// Saves the changes being tracked to storage in a single batch operation.
        /// </summary>
        public override void SaveBatchChanges()
        {
            ((TableServiceContext)Context).SaveChangesWithRetries(AddSafelyBatchOption(SaveChangesDefaultOptions));
        }
    }
}
