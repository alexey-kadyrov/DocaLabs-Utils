using DocaLabs.Storage.Core.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace DocaLabs.AzureStorage.Tables
{
    /// <summary>
    /// Defines methods for a Windows Azure table service repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public interface IAzureTableRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, ITableEntity
    {
        /// <summary>
        /// Updates the specified entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(TEntity entity);
    }
}
