
namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Implements TransientRepositoryFactory to create an instance of a transient (repository which each time uses a new database connection).
    /// using the specified session factory and CurrentPartitionProxy to get the connection. The side effect is that there will be only one repository per connection.
    /// </summary>
    public class PartitionedTransientRepositoryFactory : TransientRepositoryFactoryBase
    {
        /// <summary>
        /// Creates a new instance of a repository using a new database connection each time.
        /// </summary>
        public override ITransientRepository<TEntity> Create<TEntity>()
        {
            return new TransientRepository<TEntity>(new PartitionedDbConnectionManager());
        }
    }
}
