using System;
using System.Linq;

namespace DocaLabs.Storage.Core.Utils
{
    /// <summary>
    /// Extension helper methods tp work with instances of QueryFilter{T} class.
    /// </summary>
    public static class QueryFilterExtensions
    {
        /// <summary>
        /// Applies query filter to queryable instance. It's reverse method for the QueryFilter{T}.Apply.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <param name="target">Target for filtering.</param>
        /// <param name="filter">Filter to be applied.</param>
        /// <returns>Queryable with all filter(s) conditions applied.</returns>
        public static IQueryable<T> Apply<T>(this IQueryable<T> target, QueryFilter<T> filter)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (filter == null)
                throw new ArgumentNullException("filter");

            return filter.Apply(target);
        }
    }
}
