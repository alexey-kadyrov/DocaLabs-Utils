namespace DocaLabs.Storage.Core.Repositories
{
    /// <summary>
    /// Defines methods for a repository factory.
    /// </summary>
    public interface IRepositoryFactory : IRepositorySession
    {
        /// <summary>
        /// Creates a new instance of a repository for the specified entity type.
        /// </summary>
        IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class;
    }
}
