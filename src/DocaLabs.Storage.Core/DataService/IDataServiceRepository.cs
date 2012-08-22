namespace DocaLabs.Storage.Core.DataService
{
    /// <summary>
    /// Defines methods for a persistable repository for the data service.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public interface IDataServiceRepository<TEntity> : IRepository<TEntity> 
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Gets the current data service context.
        /// </summary>
        IDataServiceStorageContext DataService { get; }

        /// <summary>
        /// Gets the table name where entities are stored.
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Changes the state of the specified entity to Modified.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        void Update(TEntity item);
    }
}
