using System;
using System.Data.Services.Client;
using System.Linq;

namespace DocaLabs.Storage.Core.DataService
{
    /// <summary>
    /// The wrapper around runtime context of the data service.
    /// </summary>
    public class DataServiceStorageContext : IDataServiceStorageContext
    {
        /// <summary>
        /// Gets the underlying DataServiceContext
        /// </summary>
        public DataServiceContext Context { get; private set; }

        /// <summary>
        /// Gets or sets the SaveChangesOptions values that are used to save changes.
        /// </summary>
        public SaveChangesOptions SaveChangesDefaultOptions
        {
            get { return Context.SaveChangesDefaultOptions; }
            set { Context.SaveChangesDefaultOptions = value; }
        }

        /// <summary>
        /// Initializes a new instance of the DataServiceStorageContext.
        /// </summary>
        /// <param name="context">The runtime context of the data service.</param>
        public DataServiceStorageContext(DataServiceContext context)
        {
            if(context == null)
                throw new ArgumentNullException("context");

            Context = context;
        }

        /// <summary>
        /// Adds the specified object to the set of objects being tracked.
        /// </summary>
        /// <param name="tableName">The name of the table to which the resource will be added.</param>
        /// <param name="entity">The object to be tracked.</param>
        public virtual void AddObject(string tableName, object entity)
        {
            Context.AddObject(tableName, entity);
        }

        /// <summary>
        /// Changes the state of the specified object to Modified.
        /// </summary>
        /// <param name="entity">The tracked entity to be assigned to the Modified state.</param>
        public virtual void UpdateObject(object entity)
        {
            Context.UpdateObject(entity);
        }

        /// <summary>
        /// Changes the state of the specified object to be deleted.
        /// </summary>
        /// <param name="entity">The tracked entity to be changed to the deleted state.</param>
        public virtual void DeleteObject(object entity)
        {
            Context.DeleteObject(entity);
        }

        /// <summary>
        /// Creates a data service query for data of a specified generic type.
        /// </summary>
        /// <typeparam name="T">The type returned by the query.</typeparam>
        /// <param name="tableName">Table name.</param>
        /// <returns>A new DataServiceQuery(Of TElement) instance that represents a data service query.</returns>
        public virtual IQueryable<T> CreateQuery<T>(string tableName) where T : class, IEntity
        {
            return Context.CreateQuery<T>(tableName);
        }

        /// <summary>
        /// Saves the changes being tracked to storage.
        /// </summary>
        public virtual void SaveChanges()
        {
            Context.SaveChanges(SaveChangesDefaultOptions);
        }

        /// <summary>
        /// Saves the changes being tracked to storage in a single batch operation.
        /// </summary>
        public virtual void SaveBatchChanges()
        {
            Context.SaveChanges(AddSafelyBatchOption(SaveChangesDefaultOptions));
        }

        /// <summary>
        /// Adds SaveChangesOptions.Batch safely as it's cannot be combined with SaveChangesOptions.ContinueOnError.
        /// </summary>
        /// <param name="options">The original SaveChangesOptions.</param>
        /// <returns>Options with added SaveChangesOptions.Batch.</returns>
        protected static SaveChangesOptions AddSafelyBatchOption(SaveChangesOptions options)
        {
            if ((options & SaveChangesOptions.ContinueOnError) != 0)
                options ^= SaveChangesOptions.ContinueOnError;

            return options | SaveChangesOptions.Batch;
        }
    }
}