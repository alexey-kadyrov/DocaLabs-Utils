using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.NHibernate
{
    /// <summary>
    /// Provides implementation of IRepository for NHibernate.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public class Repository<TEntity> : INHibernateRepository<TEntity>
        where TEntity : class, IEntity
    {
        IQueryable<TEntity> _query;

        /// <summary>
        /// Gets the underlying session manager.
        /// </summary>
        protected ISessionManager SessionManager { get; private set; }

        /// <summary>
        /// Provides access to NHibernate's runtime session.
        /// </summary>
        public ISessionContext SessionContext
		{
			get
			{
			    return SessionManager.IsOpen 
                    ? SessionManager.Session 
                    : SessionManager.OpenSession();
			}
		}

        /// <summary>
        /// Creates a Linq query for the entity.
        /// </summary>
        protected IQueryable<TEntity> Query
        {
            get { return _query ?? (_query = SessionContext.CreateQuery<TEntity>()); }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </returns>
        public Expression Expression { get { return Query.Expression; } }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.
        /// </returns>
        public Type ElementType { get { return Query.ElementType; } }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.
        /// </returns>
        public IQueryProvider Provider { get { return Query.Provider; } }

        /// <summary>
        /// Provides access to unit of work for such operations as committing any pending changes.
        /// </summary>
        public IStorageUnit Unit
        {
            get { return SessionContext; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
		/// <param name="sessionManager">The session manager.</param>
        public Repository(ISessionManager sessionManager)
		{
			if (sessionManager == null)
				throw new ArgumentNullException("sessionManager");

			SessionManager = sessionManager;
		}

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
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
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Rereads the state of the given instance from the underlying database.
        /// </summary>
        public virtual void Refresh(object obj)
        {
            SessionContext.Refresh(obj);
        }

        /// <summary>
        /// Adds the specified entity to repository.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        public virtual void Add(TEntity item)
        {
            SessionContext.Add(item);
        }

        /// <summary>
        /// Removes the specified entity from repository.
        /// </summary>
        /// <param name="item">The entity to be deleted.</param>
        public virtual void Remove(TEntity item)
        {
            SessionContext.Remove(item);
        }

        /// <summary>
        /// Gets the entity by its primary key value.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>Returns the found entity, or null.</returns>
        public virtual TEntity Find(params object[] keyValues)
        {
            if (keyValues == null)
                throw new ArgumentNullException("keyValues");

            return SessionContext.Get<TEntity>(keyValues[0]);
        }
    }
}
