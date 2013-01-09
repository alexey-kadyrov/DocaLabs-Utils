using System;
using System.Data.Entity;
using DocaLabs.Storage.Core.Repositories;

namespace DocaLabs.EntityFrameworkStorage
{
    /// <summary>
    /// Wraps around DbContext to provide support for the Repository.
    /// </summary>
    public class RepositorySession : IDbRepositorySession
    {
        readonly IDbContextFactory _contextFactory;
        DbContext _context;

        /// <summary>
        /// Creates the context or returns already existing.
        /// </summary>
        public DbContext Context { get { return _context ?? (_context = _contextFactory.Create()); } }

        /// <summary>
        /// Initializes an instance of the RepositorySession class.
        /// </summary>
        public RepositorySession(IDbContextFactory contextFactory)
        {
            if (contextFactory == null)
                throw new ArgumentNullException("contextFactory");

            _contextFactory = contextFactory;
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
            if (disposing && _context != null)
                _context.Dispose();
        }

        /// <summary>
        /// Saves the changes being tracked to storage.
        /// </summary>
        public virtual void SaveChanges()
        {
            Context.SaveChanges();
        }

        /// <summary>
        /// Creates a new instance of a repository for the specified entity type.
        /// </summary>
        public IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(this);
        }

        /// <summary>
        /// Returns a DbSet for the specified type.
        /// </summary>
        public IDbSet<TEntity> GetSet<TEntity>() where TEntity : class
        {
            return Context.Set<TEntity>();
        }
    }
}
