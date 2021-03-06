using System;
using System.Data.Common;
using System.Data.Entity;

namespace DocaLabs.EntityFrameworkStorage
{
    /// <summary>
    /// Provides facilities for querying and working with the repository entity.
    /// </summary>
    public class RepositoryAggregateRoot<TEntity> : DbContext 
        where TEntity : class
    {
        // ReSharper disable StaticFieldInGenericType
        static readonly object Locker = new object();
        static Action<DbModelBuilder> _onModelCreatingAction;
        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Gets or sets the action to be called when the model for a derived context has been initialized.
        /// </summary>
        /// <remarks>
        /// It's intentional use of the static property/field in the generics as there must be an action per entity type.
        /// </remarks>
        public static Action<DbModelBuilder> OnModelCreatingAction
        {
            get
            {
                lock (Locker)
                {
                    return _onModelCreatingAction;
                }
            }

            set
            {
                lock (Locker)
                {
                    _onModelCreatingAction = value;
                }
            }
        }

        /// <summary>
        /// The entity set as aggregate root. The property is initialized by the DbContext.
        /// </summary>
        public IDbSet<TEntity> Entities { get; set; }

        /// <summary>
        /// Initializes a new context instance using the given string as the name or connection string for the database to which a connection will be made.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string</param>
        public RepositoryAggregateRoot(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        /// Initializes a new context instance using the existing connection to connect to a database. The connection will not be disposed when the context is disposed.
        /// </summary>
        /// <param name="connection">An existing connection to use for the new context.</param>
        /// <param name="contextOwnsConnection">If set to <c>true</c> the connection is disposed when the context is disposed, otherwise the caller must dispose the connection.</param>
        public RepositoryAggregateRoot(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, 
        /// but before the model has been locked down and used to initialize the context. 
        /// The default implementation of this method does nothing, but it can be overridden 
        /// in a derived class such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var action = OnModelCreatingAction;
            if (action != null)
                action(modelBuilder);
        }
    }
}