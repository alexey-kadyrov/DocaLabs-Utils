using System.Data.Common;
using System.Data.Entity;
using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Provides facilities for querying and working with the repository entity.
    /// </summary>
    public class DbContextAggregateRoot<TEntity> : DbContext 
        where TEntity : class, IEntity
    {
        /// <summary>
        /// The entity set as aggregate root.
        /// </summary>
        public IDbSet<TEntity> AggregateRoot { get; set; }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to a database. The connection will not be disposed when the context is disposed.
        /// </summary>
        /// <param name="connection">An existing connection to use for the new context.</param>
        public DbContextAggregateRoot(DbConnection connection)
            : base(connection, false)
        {
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, 
        /// but before the model has been locked down and used to initialize the context. 
        /// The default implementation of this method does nothing, but it can be overridden 
        /// in a derived class such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var onModelCreatingAction = RepositoryConfiguration.GetOnModelCreatingAction<TEntity>();
            if (onModelCreatingAction != null)
                onModelCreatingAction(modelBuilder);
        }
    }
}