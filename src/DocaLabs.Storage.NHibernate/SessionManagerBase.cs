using System;
using DocaLabs.Storage.Core;
using NHibernate;

namespace DocaLabs.Storage.NHibernate
{
    /// <summary>
    /// Base class to manage the NHibrnate sessions.
    /// </summary>
    public abstract class SessionManagerBase : ISessionManager
    {
        ISessionFactory SessionFactory { get; set; }
        IDbConnectionWrapper ConnectionWrapper { get; set; }

        /// <summary>
        /// Initializes an instance of the SessionManagerBase class with specified session factory.
        /// </summary>
        protected SessionManagerBase(ISessionFactory sessionFactory)
        {
            if (sessionFactory == null)
                throw new ArgumentNullException("sessionFactory");

            SessionFactory = sessionFactory;
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

            if (ConnectionWrapper != null)
                ConnectionWrapper.Dispose();

            if (Session != null)
                Session.Dispose();
        }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        public ISessionContext Session { get; private set; }

        /// <summary>
        /// Opens the session or returns already existing.
        /// </summary>
        /// <returns>The new session object.</returns>
        public ISessionContext OpenSession()
        {
            if (!IsOpen)
            {
                ConnectionWrapper = GetConnection();

                try
                {
                    ConnectionWrapper.Connection.Open();
                    Session = new SessionContext(SessionFactory.OpenSession(ConnectionWrapper.Connection));
                }
                catch
                {
                    ConnectionWrapper.Dispose();
                    ConnectionWrapper = null;
                    throw;
                }
            }

            return Session;
        }

        /// <summary>
        /// Gets a value indicating whether the session is open.
        /// </summary>
        public bool IsOpen
        {
            get { return Session != null; }
        }

        /// <summary>
        /// Gets an instance of the connection wrapper in order to open the session.
        /// </summary>
        protected abstract IDbConnectionWrapper GetConnection();
    }
}
