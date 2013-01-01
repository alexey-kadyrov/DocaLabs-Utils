using System;
using System.Collections.Generic;

namespace DocaLabs.Storage.Core.Repositories
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
        public static void AddRange<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> items) 
            where TEntity : class
        {
            if(repository == null)
                throw new ArgumentNullException("repository");

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
        public static void AddRange<TEntity>(this IRepository<TEntity> repository, params TEntity[] items)
            where TEntity : class
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            repository.AddRange((IEnumerable<TEntity>)items);
        }

        /// <summary>
        /// Removes the list of specified entities in repository.
        /// </summary>
        /// <param name="repository">Target repository.</param>
        /// <param name="items">The entities to de deleted.</param>
        public static void RemoveRange<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> items)
            where TEntity : class
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

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
        public static void RemoveRange<TEntity>(this IRepository<TEntity> repository, params TEntity[] items)
            where TEntity : class
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            repository.RemoveRange((IEnumerable<TEntity>)items);
        }
    }
}
