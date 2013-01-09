using System.Data.Entity;

namespace DocaLabs.EntityFrameworkStorage
{
    /// <summary>
    /// Defines methods for DbContext factory which is used by RepositorySession to create database contexts.
    /// </summary>
    public interface IDbContextFactory
    {
        /// <summary>
        /// Creates a new instance of a context.
        /// </summary>
        DbContext Create();
    }
}
