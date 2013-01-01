namespace DocaLabs.Storage.Core.Repositories.DataService
{
    /// <summary>
    /// Defines methods for a persistable data service repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public interface IDataServiceRepository<TEntity> : IQueryableRepository<TEntity>
        where TEntity : class 
    {
        /// <summary>
        /// Updates the specified entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(TEntity entity);
    }
}
