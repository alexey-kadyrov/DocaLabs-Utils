using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.WindowsAzure.StorageClient;

namespace DocaLabs.Storage.Azure.Tables
{
    /// <summary>
    /// The wrapper which helps to wrap the final Linq query to CloudTableQuery in order to handle the continuation token gracefully.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public class TableServiceQueryableWrapper<TEntity> : IQueryable<TEntity>
    {
        IQueryable<TEntity> Query { get; set; }

        /// <summary>
        /// Initializes the instance of the TableServiceQueryableWrapper class for specified query.
        /// </summary>
        /// <param name="query">Query that should be wrapped.</param>
        public TableServiceQueryableWrapper(IQueryable<TEntity> query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            Query = query;
            Provider = new QueryableProvider(query);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <remarks>
        /// Attempts to get all values correctly treating continuation tokens by wrapping to CloudTableQuery.
        /// </remarks>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            var serviceQuery = Query as DataServiceQuery<TEntity>;

            // in order to handle the continuation tokens the query must be wrapped by CloudTableQuery
            return serviceQuery != null
                ? new CloudTableQuery<TEntity>(serviceQuery).GetEnumerator()
                : Query.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </returns>
        public Expression Expression
        {
            get { return Query.Expression; }
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.
        /// </returns>
        public Type ElementType
        {
            get { return Query.ElementType; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.
        /// </returns>
        public IQueryProvider Provider { get; private set; }

        class QueryableProvider : IQueryProvider
        {
            IQueryable<TEntity> WrappedQuery { get; set; }

            public QueryableProvider(IQueryable<TEntity> query)
            {
                WrappedQuery = query;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TableServiceQueryableWrapper<TEntity>(WrappedQuery.Provider.CreateQuery<TEntity>(expression));
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TableServiceQueryableWrapper<TElement>(WrappedQuery.Provider.CreateQuery<TElement>(expression));
            }

            public object Execute(Expression expression)
            {
                return WrappedQuery.Provider.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return WrappedQuery.Provider.Execute<TResult>(expression);
            }
        }
    }
}
