using System.Data.Entity;
using DocaLabs.Storage.Core.Repositories;

namespace DocaLabs.EntityFrameworkStorage
{
    public interface IDbRepositorySession : IRepositoryFactory
    {
        DbContext Context { get; }
        IDbSet<TEntity> GetSet<TEntity>() where TEntity : class;
    }
}