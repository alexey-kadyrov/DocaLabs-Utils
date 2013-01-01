using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Provides implementation of IEntityFrameworkRepository and IRepository for Entity Framework.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public class Repository<TEntity> : IEntityFrameworkRepository<TEntity>
        where TEntity : class, IEntity
    {
        IDbSet<TEntity> _dbSet;
        IRepositoryContext<TEntity> _context;

        /// <summary>
        /// Gets the underlying connection manager.
        /// </summary>
        protected IDbConnectionManager DbConnectionManager { get; set; }

        /// <summary>
        /// Provides access to Entity Framework's runtime context.
        /// </summary>
        public IRepositoryContext<TEntity> RepositoryContext
		{
            get { return _context ?? (_context = DbConnectionManager.CreateRepositoryContext<TEntity>()); }
		}

        /// <summary>
        /// Gets a DbSet object for the entity.
        /// </summary>
        protected IDbSet<TEntity> DbSet
        {
            get { return _dbSet ?? (_dbSet = RepositoryContext.GetSet()); }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </returns>
        public virtual Expression Expression
        {
            get { return DbSet.Expression; }
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.
        /// </returns>
        public virtual Type ElementType
        {
            get { return DbSet.ElementType; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.
        /// </returns>
        public virtual IQueryProvider Provider
        {
            get { return DbSet.Provider; }
        }

        /// <summary>
        /// Provides access to unit of work for such operations as committing any pending changes.
        /// </summary>
        public IStorageContext Context
        {
            get { return RepositoryContext; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="dbConnectionManager">The context manager.</param>
        public Repository(IDbConnectionManager dbConnectionManager)
        {
            if (dbConnectionManager == null)
                throw new ArgumentNullException("dbConnectionManager");

            DbConnectionManager = dbConnectionManager;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public virtual IEnumerator<TEntity> GetEnumerator()
        {
            return DbSet.GetEnumerator();
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
        /// Rereads the state of the given instance from the underlying database
        /// </summary>
        public virtual void Refresh(object obj)
        {
            RepositoryContext.Refresh(obj);
        }

        /// <summary>
        /// Adds the specified entity to repository.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        public virtual void Add(TEntity item)
        {
            DbSet.Add(item);
        }

        /// <summary>
        /// Removes the specified entity from repository.
        /// </summary>
        /// <param name="item">The entity to be deleted.</param>
        public virtual void Remove(TEntity item)
        {
            DbSet.Remove(item);
        }

        /// <summary>
        /// Gets the entity by its primary key value.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>Returns the found entity, or null.</returns>
        public virtual TEntity Find(params object[] keyValues)
        {
            // as the DbSet won't throw exception and in order to make behaviour similar to NHibernate repository.
            if (keyValues == null)
                throw new ArgumentNullException("keyValues");

            return DbSet.Find(keyValues);
        }
    }
}
