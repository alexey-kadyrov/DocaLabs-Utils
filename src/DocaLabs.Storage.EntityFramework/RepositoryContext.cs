using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Wraps around DbContext to provide support for the Repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity this context is handling.</typeparam>
    public class RepositoryContext<TEntity> : IRepositoryContext<TEntity>
         where TEntity : class, IEntity
    {
        /// <summary>
        /// Gets the root context which manages the entity.
        /// </summary>
        public DbContextAggregateRoot<TEntity> Context { get; private set; }

        /// <summary>
        /// Initializes an instance of the RepositoryContext class.
        /// </summary>
        /// <param name="connection">An existing connection to use for the new context.</param>
        public RepositoryContext(DbConnection connection)
        {
            Context = new DbContextAggregateRoot<TEntity>(connection);
        }

        /// <summary>
        /// Saves the changes being tracked to storage.
        /// </summary>
        public virtual void SaveChanges()
        {
            Context.SaveChanges();
        }

        /// <summary>
        /// Rereads the state of the given instance from the underlying database
        /// </summary>
        public virtual void Refresh(object obj)
        {
            ((IObjectContextAdapter)Context).ObjectContext.Refresh(RefreshMode.StoreWins, obj);
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
            if(disposing && Context != null)
                Context.Dispose();
        }

        /// <summary>
        /// Returns a DbSet for the specified type.
        /// </summary>
        /// <returns>A DbSet instance for the given entity type.</returns>
        public virtual IDbSet<TEntity> GetSet()
        {
            return Context.AggregateRoot;
        }
    }
}
