using System.Data.Services.Client;
using System.Linq;

namespace DocaLabs.Storage.Core.DataService
{
    /// <summary>
    /// Defines methods for the runtime context of the data service.
    /// </summary>
    public interface IDataServiceStorageContext : IStorageUnit
    {
        /// <summary>
        /// Gets the underlying DataServiceContext
        /// </summary>
        DataServiceContext Context { get; }

        /// <summary>
        /// Gets or sets the SaveChangesOptions values that are used to save changes.
        /// </summary>
        SaveChangesOptions SaveChangesDefaultOptions { get; set; }

        /// <summary>
        /// Adds the specified object to the set of objects being tracked.
        /// </summary>
        /// <param name="tableName">The name of the table to which the resource will be added.</param>
        /// <param name="entity">The object to be tracked.</param>
        void AddObject(string tableName, object entity);

        /// <summary>
        /// Changes the state of the specified object to Modified.
        /// </summary>
        /// <param name="entity">The tracked entity to be assigned to the Modified state.</param>
        void UpdateObject(object entity);

        /// <summary>
        /// Changes the state of the specified object to be deleted.
        /// </summary>
        /// <param name="entity">The tracked entity to be changed to the deleted state.</param>
        void DeleteObject(object entity);

        /// <summary>
        /// Saves the changes being tracked to storage in a single batch operation.
        /// </summary>
        void SaveBatchChanges();

        /// <summary>
        /// Creates a data service query for data of a specified generic type.
        /// </summary>
        /// <typeparam name="T">The type returned by the query.</typeparam>
        /// <param name="tableName">Table name.</param>
        /// <returns>A new DataServiceQuery(Of TElement) instance that represents a data service query.</returns>
        IQueryable<T> CreateQuery<T>(string tableName) where T : class, IEntity;
    }
}
