using DocaLabs.Storage.Core.Repositories;
using NHibernate;

namespace DocaLabs.NHibernateStorage
{
    /// <summary>
    /// Implements methods to create an instance of a repository session.
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
    public class RepositorySessionFactory : IRepositorySessionFactory
    {
        readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// Initializes an instance of the RepositorySessionFactory class with specified session factory.
        /// </summary>
        public RepositorySessionFactory(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        /// <summary>
        /// Creates a new instance of a repository factory.
        /// </summary>
        public IRepositoryFactory Create()
        {
            return new RepositorySession(_sessionFactory);
        }
    }
}