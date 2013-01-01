using System;
using System.Data.Services.Client;

namespace DocaLabs.Storage.Core.Repositories.DataService
{
    /// <summary>
    /// Manages a DataService instance for a repository.
    /// </summary>
    public class DataServiceRepositorySession : IRepositorySession
    {
        /// <summary>
        /// Gets the current instance of the DataServiceContext.
        /// </summary>
        public DataServiceContext DataService { get; private set; }

        /// <summary>
        /// Initializes a new instance of the DataServiceRepositorySession class with the specified serviceRoot.
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        public DataServiceRepositorySession(Uri serviceRoot)
        {
            if(serviceRoot==null)
                throw new ArgumentNullException("serviceRoot");

            DataService = new DataServiceContext(serviceRoot)
            {
                IgnoreResourceNotFoundException = true
            };
        }

        /// <summary>
        /// Does nothing as there is no resources to dispose.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Saves the changes being tracked to storage.
        /// </summary>
        public void SaveChanges()
        {
            DataService.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);
        }

        /// <summary>
        /// Saves the changes being tracked to storage in a single batch operation.
        /// </summary>
        public virtual void SaveBatchChanges()
        {
            DataService.SaveChanges(SaveChangesOptions.ReplaceOnUpdate | SaveChangesOptions.Batch);
        }
    }
}
