using System;
using System.Collections.Generic;
using System.Linq;

namespace DocaLabs.Storage.Core.Repositories.DataService
{
    /// <summary>
    /// Provides implementation of IDataServiceRepository.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public class DataServiceRepository<TEntity> : IDataServiceRepository<TEntity> 
        where TEntity : class
    {
        readonly DataServiceRepositorySession _session;

        /// <summary>
        /// Gets the current data service session.
        /// </summary>
        public IRepositorySession Session { get { return _session; }}

        /// <summary>
        /// Gets the table name where entities are stored.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Gets IQueryable for the repository
        /// </summary>
        public IQueryable<TEntity> Query { get { return _session.DataService.CreateQuery<TEntity>(TableName); } }

        /// <summary>
        /// Initializes a new instance of the DataServiceRepository class with the specified service root and table name.
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        /// <param name="tableName">Table name, if the value is null or white space string then the table name is resolved using EntityToNameMap.</param>
        public DataServiceRepository(Uri serviceRoot, string tableName = null)
        {
            TableName = string.IsNullOrWhiteSpace(tableName)
                ? EntityToNameMap.Get<TEntity>()
                : tableName;
                
            _session = new DataServiceRepositorySession(serviceRoot);
        }

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public virtual void Add(TEntity entity)
        {
            _session.DataService.AddObject(TableName, entity);
        }

        /// <summary>
        /// Updates the specified entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public virtual void Update(TEntity entity)
        {
            _session.DataService.UpdateObject(entity);
        }

        /// <summary>
        /// Removes the specified entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        public virtual void Remove(TEntity entity)
        {
            _session.DataService.DeleteObject(entity);
        }

        /// <summary>
        /// Gets the entity by its primary key value.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>Returns the found entity, or null.</returns>
        public virtual TEntity Get(params object[] keyValues)
        {
            return Query.FindByKeys<TEntity>(keyValues);
        }

        /// <summary>
        /// Executes the configured query.
        /// </summary>
        /// <returns>The list of entities which satisfy the query.</returns>
        public IList<TEntity> Execute(IQuery<TEntity> query)
        {
            if(query == null)
                throw new ArgumentNullException("query");

            return query.Execute(this);
        }
    }
}
