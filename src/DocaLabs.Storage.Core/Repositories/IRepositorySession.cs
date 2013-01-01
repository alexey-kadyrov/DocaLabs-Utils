using System;

namespace DocaLabs.Storage.Core.Repositories
{
    /// <summary>
    /// Defines methods to manage session/unit of work for a repository.
    /// </summary>
    public interface IRepositorySession : IDisposable
    {
        /// <summary>
        /// Saves the changes being tracked to storage.
        /// </summary>
        void SaveChanges();
    }
}
