using System;
using System.Linq;

namespace DocaLabs.Storage.Core.Utils
{
    /// <summary>
    /// Defines base class for cascading filters on queryable collection/repository.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class QueryFilter<T>
    {
        QueryFilter<T> Next { get; set; }

        /// <summary>
        /// Helper method for filter chaining.
        /// </summary>
        /// <param name="filters">Filters to be chained.</param>
        /// <returns>Itself.</returns>
        public QueryFilter<T> Chain(params QueryFilter<T>[] filters)
        {
            if(filters == null || filters.Length == 0 || filters.Any(f => f == null))
                throw new ArgumentNullException("filters");

            var current = this;

            foreach (var filter in filters)
            {
                current.Next = filter;
                current = filter;
            }

            return this;
        }

        /// <summary>
        /// Applies the filter and the chain if defined to the queryable collection/repository. 
        /// </summary>
        /// <param name="target">Target for filtering.</param>
        /// <returns>Queryable with all filter(s) conditions applied.</returns>
        public IQueryable<T> Apply(IQueryable<T> target)
        {
            if(target == null)
                throw new ArgumentNullException("target");

            var result = Applying(target);

            return Next != null ? Next.Apply(result) : result;
        }

        /// <summary>
        /// Applies this filter conditions.
        /// </summary>
        /// <param name="target">Target for filtering.</param>
        /// <returns>Queryable this filter conditions applied.</returns>
        protected abstract IQueryable<T> Applying(IQueryable<T> target);
    }
}
