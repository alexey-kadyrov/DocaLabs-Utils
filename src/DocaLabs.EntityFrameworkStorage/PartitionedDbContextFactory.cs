using System;
using System.Data.Entity;
using DocaLabs.Storage.Core;

namespace DocaLabs.EntityFrameworkStorage
{
    public class PartitionedDbContextFactory<TContext> : IDbContextFactory
        where TContext : DbContext
    {
        readonly IDatabaseConnection _connection;

        public PartitionedDbContextFactory(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public DbContext Create()
        {
            return (DbContext)Activator.CreateInstance(typeof(TContext), _connection.Connection);
        }
    }
}
