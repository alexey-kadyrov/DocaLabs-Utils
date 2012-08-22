using System;

namespace DocaLabs.Storage.NHibernate
{
    /// <summary>
    /// Manages the NHibrnate sessions.
    /// </summary>
    public interface ISessionManager : IDisposable
    {
        /// <summary>
        /// Gets the current session.
        /// </summary>
        ISessionContext Session { get; }

        /// <summary>
        /// Opens the session or returns already existing.
        /// </summary>
        /// <returns>The new session object.</returns>
        ISessionContext OpenSession();

        /// <summary>
        /// Gets a value indicating whether the session is open.
        /// </summary>
        bool IsOpen { get; }
    }
}
