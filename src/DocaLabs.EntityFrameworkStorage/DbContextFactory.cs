using System;
using System.Data.Entity;

namespace DocaLabs.EntityFrameworkStorage
{
    /// <summary>
    /// Implements DbContext factory which uses either the database name or a connection string.
    /// </summary>
    public class DbContextFactory<TContext> : IDbContextFactory
        where TContext : DbContext
    {
        readonly string _nameOrConnectionString;

        /// <summary>
        /// Initializes an instance of the DbContextFactory class with either the database name or a connection string.
        /// </summary>
        public DbContextFactory(string nameOrConnectionString)
        {
            _nameOrConnectionString = nameOrConnectionString;
        }

        /// <summary>
        /// Creates a new instance of a context.
        /// </summary>
        public virtual DbContext Create()
        {
            return (DbContext)Activator.CreateInstance(typeof(TContext), _nameOrConnectionString);
        }
    }
}
