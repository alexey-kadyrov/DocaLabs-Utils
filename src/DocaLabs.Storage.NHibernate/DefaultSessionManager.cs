using System;
using DocaLabs.Storage.Core;
using NHibernate;

namespace DocaLabs.Storage.NHibernate
{
    /// <summary>
    /// Manages the NHibrnate sessions using provided connection wrapper and session factory.
    /// </summary>
    /// <remarks>
    /// The constructor accepts IDbConnectionWrapper instead of DbConnectionString in order to enable
    /// scenarios like Sql Azure Federations.  
    /// </remarks>
    public class DefaultSessionManager : SessionManagerBase
    {
        IDbConnectionWrapper ConnectionWrapper { get; set; }

        /// <summary>
        /// Initializes an instance of the DefaultSessionManager class with specified session factory and connection string.
        /// </summary>
        public DefaultSessionManager(ISessionFactory sessionFactory, IDbConnectionWrapper connectionWrapper)
            : base(sessionFactory)
        {
            if(connectionWrapper == null)
                throw new ArgumentNullException("connectionWrapper");

            ConnectionWrapper = connectionWrapper;
        }

        /// <summary>
        /// Gets an instance of the connection wrapper in order to open the session.
        /// </summary>
        protected override IDbConnectionWrapper GetConnection()
        {
            return ConnectionWrapper;
        }
    }
}
