using System;
using System.Data.Entity;
using DocaLabs.Storage.Core.Repositories;

namespace DocaLabs.EntityFrameworkStorage
{
    public abstract class ScalarQuery<TEntity, TResult> : IScalarQuery<TEntity, TResult>
        where TEntity : class 
    {
        public TResult Execute(IRepository<TEntity> repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            return Execute(((IDbRepositorySession)repository.Session).Context);
        }

        protected abstract TResult Execute(DbContext context);
    }
}
