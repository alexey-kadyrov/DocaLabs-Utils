using System;
using System.Data.Entity;

namespace DocaLabs.EntityFrameworkStorage
{
    /// <summary>
    /// Base class to manage IDatabaseConnection instances.
    /// </summary>
    public class DbContextFactory<TContext> : IDbContextFactory
        where TContext : DbContext
    {
        readonly string _nameOrConnectionString;

        /// <summary>
        /// Initializes an instance of the DbContextFactory class with provided connection.
        /// </summary>
        public DbContextFactory(string nameOrConnectionString)
        {
            _nameOrConnectionString = nameOrConnectionString;
        }

        /// <summary>
        /// Creates a new instance of the repository context.
        /// </summary>
        public virtual DbContext Create()
        {
            return (DbContext)Activator.CreateInstance(typeof(TContext), _nameOrConnectionString);
        }
    }
}
