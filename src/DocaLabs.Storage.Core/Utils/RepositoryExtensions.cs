using System;
using System.Collections.Generic;

namespace DocaLabs.Storage.Core.Utils
{
    /// <summary>
    /// Defines extensions methods for repositories.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Adds the list of specified entities to repository.
        /// </summary>
        /// <param name="repository">Target repository.</param>
        /// <param name="items">The entities to add.</param>
        public static void AddRange<TEntity>(this IStorageSet<TEntity> repository, IEnumerable<TEntity> items) 
            where TEntity : class, IEntity
        {
            if (items == null)
                throw new ArgumentNullException("items");

            foreach (var item in items)
                repository.Add(item);
        }

        /// <summary>
        /// Adds the list of specified entities to repository.
        /// </summary>
        /// <param name="repository">Target repository.</param>
        /// <param name="items">The entities to add.</param>
        public static void AddRange<TEntity>(this IStorageSet<TEntity> repository, params TEntity[] items)
            where TEntity : class, IEntity
        {
            repository.AddRange((IEnumerable<TEntity>)items);
        }

        /// <summary>
        /// Removes the list of specified entities in repository.
        /// </summary>
        /// <param name="repository">Target repository.</param>
        /// <param name="items">The entities to de deleted.</param>
        public static void RemoveRange<TEntity>(this IStorageSet<TEntity> repository, IEnumerable<TEntity> items)
            where TEntity : class, IEntity
        {
            if (items == null)
                throw new ArgumentNullException("items");

            foreach (var item in items)
                repository.Remove(item);
        }

        /// <summary>
        /// Removes the list of specified entities in repository.
        /// </summary>
        /// <param name="repository">Target repository.</param>
        /// <param name="items">The entities to de deleted.</param>
        public static void RemoveRange<TEntity>(this IStorageSet<TEntity> repository, params TEntity[] items)
            where TEntity : class, IEntity
        {
            repository.RemoveRange((IEnumerable<TEntity>)items);
        }
    }
}
