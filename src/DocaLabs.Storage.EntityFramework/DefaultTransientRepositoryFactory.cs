using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Implements TransientRepositoryFactory to create an instance of a transient (repository which each time uses a new database connection).
    /// using the specified session factory and connections string. The side effect is that there will be only one repository per connection.
    /// </summary>
    public class DefaultTransientRepositoryFactory : TransientRepositoryFactoryBase
    {
        DatabaseConnectionString ConnectionString { get; set; }

        /// <summary>
        /// Initializes an instance of the TransientRepositoryFactory class with specified connection string.
        /// The class accepts the DatabaseConnectionString instance instead of IDatabaseConnection (as the DefaultSessionManager does)
        /// in order to be able to establish a new database connection for each repository.
        /// </summary>
        public DefaultTransientRepositoryFactory(DatabaseConnectionString connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Creates a new instance of a repository using a new database connection each time.
        /// </summary>
        public override ITransientRepository<TEntity> Create<TEntity>()
        {
            return new TransientRepository<TEntity>(new DefaultDbConnectionManager(new DatabaseConnection(ConnectionString)));
        }
    }
}
