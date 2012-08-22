namespace DocaLabs.Storage.Core
{
    /// <summary>
    /// Defines methods for a persistable repository for entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public interface IRepository<TEntity> : IStorageSet<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Provides access to unit of work for such operations as committing any pending changes.
        /// </summary>
        IStorageUnit Unit { get; }
    }
}
