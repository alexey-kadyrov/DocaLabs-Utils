using System.Linq;

namespace DocaLabs.Storage.Core.Repositories
{
    /// <summary>
    /// Defines methods for a repository which entities can be queried using Linq.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public interface IQueryableRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Gets IQueryable for the repository.
        /// </summary>
        IQueryable<TEntity> Query { get; }
    }
}
