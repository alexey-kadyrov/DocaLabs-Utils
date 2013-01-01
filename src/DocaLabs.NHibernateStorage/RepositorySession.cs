using System;
using DocaLabs.Storage.Core.Repositories;
using NHibernate;

namespace DocaLabs.NHibernateStorage
{
    /// <summary>
    /// Manages the NHibrnate sessions using provided session factory.
    /// </summary>
    /// <remarks>
    /// The RepositorySession is more suited when a data model is used in a single database.
    /// The ParttionedRespositorySession is more suited when the same data model is used in several databases (partitions/shards)
    /// </remarks>
    public class RepositorySession : INHibernateRepositorySession
    {
        readonly ISessionFactory _sessionFactory;
        ISession _session;

        /// <summary>
        /// Opens the session or returns already existing.
        /// </summary>
        /// <returns>The new session object.</returns>
        public ISession NHibernateSession
        {
            get { return _session ?? (_session = _sessionFactory.OpenSession()); }
        }

        /// <summary>
        /// Initializes an instance of the RepositorySession class with specified session factory.
        /// </summary>
        public RepositorySession(ISessionFactory sessionFactory)
        {
            if (sessionFactory == null)
                throw new ArgumentNullException("sessionFactory");

            _sessionFactory = sessionFactory;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases the resources used by the component.
        /// </summary>
        /// <param name="disposing">true to release resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if(disposing && _session != null)
                _session.Dispose();
        }

        /// <summary>
        /// Saves the changes being tracked to storage.
        /// </summary>
        public virtual void SaveChanges()
        {
            NHibernateSession.Flush();
        }

        /// <summary>
        /// Creates a new instance of a repository.
        /// </summary>
        public virtual IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(this);
        }
    }
}