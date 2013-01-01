namespace DocaLabs.Storage.Core.Repositories
{
    /// <summary>
    /// Defines methods to create an instance of a repository session.
    /// </summary>
    /// <remarks>
    /// The interface is designed mostly for situations when there is no clear life cycle events 
    /// so it's impossible to hook into them in order to clear resources like in case of
    /// doing some work in multiple pooled threads (either directly or through TPL).
    /// Or it can be used in situations when there is need to control precisely 
    /// when a database operation is done and the connection is disposed.
    /// The pattern would be like below:
    ///     using (var scope = new TransactionScope())
    ///     using (var session = repository_session_factory.Create())
    ///     {
    ///         var books = session.CreateRepository&lt;Book>();
    ///         // .. do some work
    ///         books.Session.SaveChanges();
    ///         scope.Complete();
    ///     }
    /// </remarks>
    public interface IRepositorySessionFactory
    {
        /// <summary>
        /// Creates a new instance of a repository factory.
        /// </summary>
        IRepositoryFactory Create();
    }
}
