using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DocaLabs.Storage.Core.Repositories;

namespace DocaLabs.EntityFrameworkStorage
{
    /// <summary>
    /// Provides implementation of IEntityFrameworkRepository and IRepository for Entity Framework.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public class Repository<TEntity> : IQueryableRepository<TEntity>
        where TEntity : class
    {
        readonly IDbRepositorySession _session;

        /// <summary>
        /// Provides access to unit of work for such operations as committing any pending changes.
        /// </summary>
        public IRepositorySession Session
        {
            get { return _session; }
        }

        public IQueryable<TEntity> Query { get { return RootSet; } }

        public IDbSet<TEntity> RootSet { get { return _session.GetSet<TEntity>(); } }

        /// <summary>
        /// Initializes a new instance of the Repository class for Entity Framework.
        /// </summary>
        /// <param name="session">The underlying repository session.</param>
        public Repository(IDbRepositorySession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            _session = session;
        }

        /// <summary>
        /// Adds the specified entity to repository.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        public virtual void Add(TEntity item)
        {
            RootSet.Add(item);
        }

        /// <summary>
        /// Removes the specified entity from repository.
        /// </summary>
        /// <param name="item">The entity to be deleted.</param>
        public virtual void Remove(TEntity item)
        {
            RootSet.Remove(item);
        }

        public TEntity Get(params object[] keyValues)
        {
            // as the DbSet won't throw exception and in order to make behaviour similar to NHibernate repository.
            if (keyValues == null)
                throw new ArgumentNullException("keyValues");

            return RootSet.Find(keyValues);
        }

        public IList<TEntity> Execute(IQuery<TEntity> query)
        {
            if(query == null)
                throw new ArgumentNullException("query");

            return query.Execute(this);
        }

        public TResult Execute<TResult>(IScalarQuery<TEntity, TResult> query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            return query.Execute(this);
        }
    }
}
