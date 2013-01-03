namespace DocaLabs.Storage.Core.Repositories
{
    /// <summary>
    /// Defines methods to execute a query on a repository which gives a single value as a result.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    /// <typeparam name="TResult">The result's type.</typeparam>
    public interface IScalarQuery<TEntity, out TResult>
        where TEntity : class
    {
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="repository">Repository which provides data.</param>
        /// <returns>A single value result.</returns>
        TResult Execute(IRepository<TEntity> repository);
    }
}
