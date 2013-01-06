using System;
using System.Collections.Generic;
using System.Data.Entity;
using DocaLabs.Storage.Core.Repositories;

namespace DocaLabs.EntityFrameworkStorage
{
    public abstract class Query<TEntity> : IQuery<TEntity>
        where TEntity : class
    {
        public IList<TEntity> Execute(IRepository<TEntity> repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            return Execute(((IDbRepositorySession)repository.Session).Context);
        }

        protected abstract IList<TEntity> Execute(DbContext context);
    }
}
