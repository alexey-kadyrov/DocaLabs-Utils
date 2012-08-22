using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DocaLabs.Storage.Core.Utils;

namespace DocaLabs.Storage.Core.DataService
{
    /// <summary>
    /// Provides implementation of IDataServiceRepository for DataServiceContext.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public class DataServiceRepository<TEntity> : IDataServiceRepository<TEntity> 
        where TEntity : class, IEntity
    {
        IQueryable<TEntity> _query;

        IDataServiceStorageContextManager ContextManager { get; set; }

        /// <summary>
        /// Gets the current data service context.
        /// </summary>
        public IDataServiceStorageContext DataService
        {
            get
            {
                return ContextManager.IsInitialized 
                    ? ContextManager.DataService 
                    : ContextManager.InitializeDataService();
            }
        }

        /// <summary>
        /// Gets the table name where entities are stored.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Gets the current IQueryable{TEntity}.
        /// </summary>
        protected IQueryable<TEntity> Query
        {
            get { return _query ?? (_query = InitializeQuery()); }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </returns>
        public virtual Expression Expression
        {
            get { return Query.Expression; }
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.
        /// </returns>
        public virtual Type ElementType
        {
            get { return Query.ElementType; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.
        /// </returns>
        public virtual IQueryProvider Provider
        {
            get { return Query.Provider; }
        }

        /// <summary>
        /// Provides access to unit of work for such operations as committing any pending changes.
        /// </summary>
        public virtual IStorageUnit Unit
        {
            get { return DataService; }
        }

        /// <summary>
        /// Initializes a new instance of the DataServiceRepository class with the specified context manager.
        /// The table name is resolved from the entity type by using the static method EntityTableNameResolver.Resolve(of TEntity).
        /// </summary>
        /// <param name="contextManager">Context manager.</param>
        public DataServiceRepository(IDataServiceStorageContextManager contextManager)
            : this(contextManager, EntityTableNameResolver.Resolve<TEntity>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataServiceRepository class with the specified context manager and table name.
        /// </summary>
        /// <param name="contextManager">Context manager.</param>
        /// <param name="tableName">Table name.</param>
        public DataServiceRepository(IDataServiceStorageContextManager contextManager, string tableName)
        {
            if (contextManager == null)
                throw new ArgumentNullException("contextManager");

            if(string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException("tableName");

            ContextManager = contextManager;
            TableName = tableName;
        }

        /// <summary>
        /// The method is called internally by the Query property to create the service query when necessary.
        /// </summary>
        /// <returns>The new query.</returns>
        protected virtual IQueryable<TEntity> InitializeQuery()
        {
            return DataService.CreateQuery<TEntity>(TableName);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public virtual IEnumerator<TEntity> GetEnumerator()
        {
            return Query.GetEnumerator();
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
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        public virtual void Add(TEntity item)
        {
            DataService.AddObject(TableName, item);
        }

        /// <summary>
        /// Updates the specified entity in the repository.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        public virtual void Update(TEntity item)
        {
            DataService.UpdateObject(item);
        }

        /// <summary>
        /// Removes the specified entity from the repository.
        /// </summary>
        /// <param name="item">The entity to be deleted.</param>
        public virtual void Remove(TEntity item)
        {
            DataService.DeleteObject(item);
        }

        /// <summary>
        /// Gets the entity by its primary key value.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>Returns the found entity, or null.</returns>
        public virtual TEntity Find(params object[] keyValues)
        {
            return Query.FindByKeys<TEntity>(keyValues);
        }
    }
}
