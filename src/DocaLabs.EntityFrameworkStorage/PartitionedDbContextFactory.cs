using System;
using System.Data.Entity;
using DocaLabs.Storage.Core.Partitioning;

namespace DocaLabs.EntityFrameworkStorage
{
    /// <summary>
    /// Implements the DbContext factory using partition proxy.
    /// Expects that TContext will implement TContext(DbConnection connection, bool contextOwnsConnection) constructor.
    /// The Create method will pass _partitionProxy.GetConnection(), true parameters to the constructor.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class PartitionedDbContextFactory<TContext> : IDbContextFactory
        where TContext : DbContext
    {
        readonly IPartitionProxy _partitionProxy;

        /// <summary>
        /// Initializes a new instance of the PartitionedDbContextFactory class using provided partition proxy.
        /// </summary>
        public PartitionedDbContextFactory(IPartitionProxy partitionProxy)
        {
            _partitionProxy = partitionProxy;
        }

        /// <summary>
        /// Creates a new instance of a context.
        /// </summary>
        public DbContext Create()
        {
            return (DbContext)Activator.CreateInstance(typeof(TContext), _partitionProxy.GetConnection().Connection, true);
        }
    }
}
