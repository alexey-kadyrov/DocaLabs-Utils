using System;
using System.Collections.Generic;
using System.Linq;
using DocaLabs.Storage.Core.Repositories;
using NHibernate.Linq;

namespace DocaLabs.NHibernateStorage
{
    /// <summary>
    /// Provides implementation of IRepository for NHibernate.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public class Repository<TEntity> : IQueryableRepository<TEntity>
        where TEntity : class
    {
        readonly INHibernateRepositorySession _session;

        /// <summary>
        /// Gets IQueryable for the repository
        /// </summary>
        public IQueryable<TEntity> Query { get { return _session.NHibernateSession.Query<TEntity>(); } }

        /// <summary>
        /// Provides access to unit of work for such operations as committing any pending changes.
        /// </summary>
        public IRepositorySession Session { get { return _session; } }

        /// <summary>
        /// Initializes a new instance of the Repository class.
        /// </summary>
		/// <param name="session">The session manager.</param>
        public Repository(INHibernateRepositorySession session)
		{
			if (session == null)
				throw new ArgumentNullException("session");

            _session = session;
		}

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public virtual void Add(TEntity entity)
        {
            _session.NHibernateSession.Save(entity);
        }

        /// <summary>
        /// Removes the specified entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        public virtual void Remove(TEntity entity)
        {
            _session.NHibernateSession.Delete(entity);
        }

        /// <summary>
        /// Gets the entity by its primary key value.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>Returns the found entity, or null.</returns>
        public virtual TEntity Get(params object[] keyValues)
        {
            if (keyValues == null)
                throw new ArgumentNullException("keyValues");

            if(keyValues.Length != 1)
                throw new ArgumentException(Resources.Text.key_must_have_only_single_value, "keyValues");

            return _session.NHibernateSession.Get<TEntity>(keyValues[0]);
        }

        /// <summary>
        /// Executes the configured query.
        /// </summary>
        /// <returns>The list of entities which satisfy the query.</returns>
        public IList<TEntity> Execute(IQuery<TEntity> query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            return query.Execute(this);
        }

        /// <summary>
        /// Executes the configured query which gives a list of entities as a result.
        /// </summary>
        /// <returns>The result of the query.</returns>
        public TResult Execute<TResult>(IScalarQuery<TEntity, TResult> query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            return query.Execute(this);
        }
    }
}
