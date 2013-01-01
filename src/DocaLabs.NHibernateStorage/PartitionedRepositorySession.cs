using System;
using System.Data;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Repositories;
using NHibernate;

namespace DocaLabs.NHibernateStorage
{
    /// <summary>
    /// Manages the NHibrnate sessions using provided connection and session factory.
    /// </summary>
    /// <remarks>
    /// The PartitionedRepositorySession is more suited when the same data model is used in several databases (partitions/shards)
    /// The RespositorySession is suited when a data model is used in single database.
    /// </remarks>
    public class PartitionedRepositorySession : INHibernateRepositorySession
    {
        readonly ISessionFactory _sessionFactory;
        readonly IDatabaseConnection _connection;
        bool _selfOpenConnection;
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
                        if (_connection.Connection.State == ConnectionState.Closed)
                        {
                            _connection.Connection.Open();
                            _selfOpenConnection = true;
                        }

                        _session = _sessionFactory.OpenSession(_connection.Connection);
                    }
                    catch
                    {
                        if (_selfOpenConnection)
                        {
                            _connection.Connection.Close();
                            _selfOpenConnection = false;
                        }

                        throw;
                    }
                }

                return _session;
            }
        }

        /// <summary>
        /// Initializes an instance of the PartitionedRepositorySession class with specified session factory and connection string.
        /// </summary>
        public PartitionedRepositorySession(ISessionFactory sessionFactory, IDatabaseConnection connection)
        {
            if (sessionFactory == null)
                throw new ArgumentNullException("sessionFactory");

            if (connection == null)
                throw new ArgumentNullException("connection");

            _sessionFactory = sessionFactory;
            _connection = connection;
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
                if (_selfOpenConnection)
                {
                    _connection.Connection.Close();
                    _selfOpenConnection = false;
                }
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
