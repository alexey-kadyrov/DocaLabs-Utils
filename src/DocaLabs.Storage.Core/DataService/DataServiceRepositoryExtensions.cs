using System;
using System.Collections.Generic;

namespace DocaLabs.Storage.Core.DataService
{
    /// <summary>
    /// Defines extensions methods specific for data service repositories.
    /// </summary>
    public static class DataServiceRepositoryExtensions
    {
        /// <summary>
        /// Updates the list of specified entities in repository.
        /// </summary>
        /// <param name="repository">Target repository.</param>
        /// <param name="items">The entities to update.</param>
        public static void UpdateRange<TEntity>(this IDataServiceRepository<TEntity> repository, IEnumerable<TEntity> items)
            where TEntity : class , IEntity
        {
            if (items == null)
                throw new ArgumentNullException("items");

            foreach (var item in items)
                repository.Update(item);
        }

        /// <summary>
        /// Updates the list of specified entities in repository.
        /// </summary>
        /// <param name="repository">Target repository.</param>
        /// <param name="items">The entities to update.</param>
        public static void UpdateRange<TEntity>(this IDataServiceRepository<TEntity> repository, params TEntity[] items)
            where TEntity : class , IEntity
        {
            repository.UpdateRange((IEnumerable<TEntity>)items);
        }
    }
}
