using DocaLabs.Storage.Core.Repositories;
using NHibernate;

namespace DocaLabs.NHibernateStorage
{
    /// <summary>
    /// Defines methods to manage NHibrnate sessions.
    /// </summary>
    public interface INHibernateRepositorySession : IRepositoryFactory
    {
        /// <summary>
        /// Opens the session or returns already existing.
        /// </summary>
        /// <returns>The session object.</returns>
        ISession NHibernateSession { get; }
    }
}
