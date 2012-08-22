using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Defines methods for Entity Framework repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public interface IEntityFrameworkRepository<TEntity> : IRefreshableRepository<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Provides access to Entity Framework's runtime context.
        /// </summary>
        IRepositoryContext<TEntity> RepositoryContext { get; }
    }
}
