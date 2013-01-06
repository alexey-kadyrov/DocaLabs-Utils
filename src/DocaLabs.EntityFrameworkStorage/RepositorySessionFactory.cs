using DocaLabs.Storage.Core.Repositories;

namespace DocaLabs.EntityFrameworkStorage
{
    public class RepositorySessionFactory : IRepositorySessionFactory
    {
        readonly IDbContextFactory _contextFactory;

        public RepositorySessionFactory(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public IRepositoryFactory Create()
        {
            return new RepositorySession(_contextFactory);
        }
    }
}
