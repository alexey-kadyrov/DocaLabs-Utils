using System;
using System.Collections.Concurrent;
using System.Data.Entity;
using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Defines configuration for repositories.
    /// </summary>
    public static class RepositoryConfiguration
    {
        static readonly ConcurrentDictionary<Type, Action<DbModelBuilder>> OnModelCreatingActions;

        static RepositoryConfiguration()
        {
            OnModelCreatingActions = new ConcurrentDictionary<Type, Action<DbModelBuilder>>();
        }

        /// <summary>
        /// Removes database initializer for a given entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity the repository is handling.</typeparam>
        public static void RemoveInitializer<TEntity>() 
            where TEntity : class, IEntity
        {
            Database.SetInitializer<DbContextAggregateRoot<TEntity>>(null);
        }

        /// <summary>
        /// Sets callback which will be called when a model is being built for the given entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity the repository is handling.</typeparam>
        /// <param name="onModelCreatingAction">
        /// Callback which will be called when a model is being created by DbContext or null if you don't want to be notified.
        /// The callback can be used to do additional fluent configuration.
        /// </param>
        public static void SetOnModelCreatingAction<TEntity>(Action<DbModelBuilder> onModelCreatingAction)
            where TEntity : class, IEntity
        {
            OnModelCreatingActions[typeof(TEntity)] = onModelCreatingAction;
        }

        /// <summary>
        /// Gets callback which will be called when a model is being built for the given entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity the repository is handling.</typeparam>
        /// <returns>
        /// Callback which will be called when a model is being created by DbContext or null if you don't want to be notified.
        /// The callback can be used to do additional fluent configuration.
        /// </returns>
        public static Action<DbModelBuilder> GetOnModelCreatingAction<TEntity>()
            where TEntity : class, IEntity
        {
            Action<DbModelBuilder> action;
            return OnModelCreatingActions.TryGetValue(typeof(TEntity), out action) ? action : null;
        }
    }
}
