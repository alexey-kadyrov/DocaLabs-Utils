using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.NHibernate
{
    /// <summary>
    /// Defines methods for NHibernate repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity this repository is handling.</typeparam>
    public interface INHibernateRepository<TEntity> : IRefreshableRepository<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Provides access to NHibernate's runtime session.
        /// </summary>
        ISessionContext SessionContext { get; }
    }
}
