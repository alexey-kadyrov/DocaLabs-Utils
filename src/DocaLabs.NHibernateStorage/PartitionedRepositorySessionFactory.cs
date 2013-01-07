using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Repositories;
using NHibernate;

namespace DocaLabs.NHibernateStorage
{
    /// <summary>
    /// Implements methods to create an instance of a partitioned repository session.
    /// </summary>
    /// <remarks>
    /// The class is mostly for situations when there is no clear life cycle events 
    /// so it's impossible to hook into them in order to clear resources like in case of
    /// doing some work in multiple pooled threads (either directly or through TPL).
    /// Or it can be used in situations when there is need to control precisely 
    /// when a database operation is done and the connection is closed.
    /// The pattern is like below:
    ///     using (var scope = new TransactionScope())
    ///     using (var session = repository_session_factory.Create())
    ///     {
    ///         var books = session.CreateRepository&lt;Book>();
    ///         // .. do some work
    ///         books.Session.SaveChanges();
    ///         scope.Complete();
    ///     }
    /// </remarks>
    public class PartitionedRepositorySessionFactory : IRepositorySessionFactory
    {
        readonly ISessionFactory _sessionFactory;
        readonly IPartitionProxy _partitionProxy;

        /// <summary>
        /// Initializes an instance of the PartitionedRepositorySessionFactory class with specified session factory and partition proxy.
        /// </summary>
        public PartitionedRepositorySessionFactory(ISessionFactory sessionFactory, IPartitionProxy partitionProxy)
        {
            _sessionFactory = sessionFactory;
            _partitionProxy = partitionProxy;
        }

        /// <summary>
        /// Creates a new instance of a repository factory.
        /// </summary>
        public IRepositoryFactory Create()
        {
            return new PartitionedRepositorySession(_sessionFactory, _partitionProxy);
        }
    }
}
