using System;
using System.Data;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Repositories;
using NHibernate;

namespace DocaLabs.NHibernateStorage
{
    /// <summary>
    /// Manages the NHibrnate sessions using provided partition proxy and session factory.
    /// </summary>
    /// <remarks>
    /// The PartitionedRepositorySession is more suited when the same data model is used in several databases (partitions/shards)
    /// The RespositorySession is suited when a data model is used in single database.
    /// </remarks>
    public class PartitionedRepositorySession : INHibernateRepositorySession
    {
        readonly ISessionFactory _sessionFactory;
        readonly IPartitionProxy _partitionProxy;
        IDatabaseConnection _connection;
        ISession _session;

        /// <summary>
        /// Opens the session or returns already existing.
        /// </summary>
        /// <returns>The new session object.</returns>
        public ISession NHibernateSession
        {
            get
            {
                if (_session == null)
                {
                    try
                    {
                        _connection = _partitionProxy.GetConnection();

                        if (_connection.Connection.State == ConnectionState.Closed)
                            _connection.Connection.Open();

                        _session = _sessionFactory.OpenSession(_connection.Connection);
                    }
                    catch
                    {
                        if (_connection != null)
                            _connection.Dispose();

                        throw;
                    }
                }

                return _session;
            }
        }

        /// <summary>
        /// Initializes an instance of the PartitionedRepositorySession class with specified session factory and partition proxy.
        /// </summary>
        public PartitionedRepositorySession(ISessionFactory sessionFactory, IPartitionProxy partitionProxy)
        {
            if (sessionFactory == null)
                throw new ArgumentNullException("sessionFactory");

            if (partitionProxy == null)
                throw new ArgumentNullException("partitionProxy");

            _sessionFactory = sessionFactory;
            _partitionProxy = partitionProxy;
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
            if(!disposing)
                return;

            try
            {
                if (_session != null)
                    _session.Dispose();
            }
            finally 
            {
                if (_connection != null)
                    _connection.Dispose();
            }
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
