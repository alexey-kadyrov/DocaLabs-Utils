namespace DocaLabs.Storage.Core
{
    /// <summary>
    /// Defines methods for a persistable repository for entities which can refresh an entity from the underlying store.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public interface IRefreshableRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Rereads the state of the given instance from the underlying store.
        /// </summary>
        void Refresh(object obj);
    }
}
