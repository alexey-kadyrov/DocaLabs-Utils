using System.Collections.Generic;

namespace DocaLabs.Storage.Core.Repositories
{
    /// <summary>
    /// Defines methods to execute a query on a repository which gives a list of entities as a result.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public interface IQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="repository">Repository which provides data.</param>
        /// <returns>The list of entities which satisfy the query.</returns>
        IList<TEntity> Execute(IRepository<TEntity> repository);
    }
}
