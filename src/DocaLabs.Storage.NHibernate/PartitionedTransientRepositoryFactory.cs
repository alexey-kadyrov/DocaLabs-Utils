using NHibernate;

namespace DocaLabs.Storage.NHibernate
{
    /// <summary>
    /// Implements TransientRepositoryFactory to create an instance of a transient (repository which each time uses a new database connection).
    /// using the specified session factory and CurrentPartitionProxy to get the connection. The side effect is that there will be only one repository per connection.
    /// </summary>
    public class PartitionedTransientRepositoryFactory : TransientRepositoryFactoryBase
    {
        /// <summary>
        /// Initializes an instance of the TransientRepositoryFactory class with specified session factory.
        /// </summary>
        public PartitionedTransientRepositoryFactory(ISessionFactory sessionFactory) 
            : base(sessionFactory)
        {
        }

        /// <summary>
        /// Creates a new instance of a repository using a new database connection each time.
        /// </summary>
        public override ITransientRepository<TEntity> Create<TEntity>()
        {
            return new TransientRepository<TEntity>(new PartitionedSessionManager(SessionFactory));
        }
    }
}
