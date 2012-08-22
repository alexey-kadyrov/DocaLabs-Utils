using System;
using System.Collections.Generic;
using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.EntityFramework
{
    /// <summary>
    /// Base class to manage IDbConnectionWrapper instances.
    /// </summary>
    public abstract class DbConnectionManagerBase : IDbConnectionManager
    {
        List<IDisposable> ObjectsToDispose { get; set; }

        /// <summary>
        /// Gets the current instance of the connection.
        /// </summary>
        public IDbConnectionWrapper Connection { get; protected set; }

        /// <summary>
        /// Initializes an instance of the DbConnectionManagerBase class with null connection.
        /// </summary>
        protected DbConnectionManagerBase()
        {
            ObjectsToDispose = new List<IDisposable>();
        }

        /// <summary>
        /// Initializes an instance of the DbConnectionManagerBase class with provided connection.
        /// </summary>
        protected DbConnectionManagerBase(IDbConnectionWrapper connection) : this()
        {
            if(connection == null)
                throw new ArgumentNullException("connection");

            Connection = connection;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases the resources used by the component.
        /// </summary>
        /// <param name="disposing">true to release resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if(!disposing)
                return;

            if(Connection != null)
                Connection.Dispose();

            foreach (var disposable in ObjectsToDispose)
                disposable.Dispose();
        }

        /// <summary>
        /// Opens the context or returns already existing.
        /// </summary>
        /// <returns>The new session object.</returns>
        public abstract IDbConnectionWrapper OpenConnection();

        /// <summary>
        /// Gets a value indicating whether the context is open.
        /// </summary>
        public bool IsOpen
        {
            get { return Connection != null; }
        }

        /// <summary>
        /// Creates a new instance of the repository context.
        /// </summary>
        public IRepositoryContext<TEntity> CreateRepositoryContext<TEntity>() where TEntity : class, IEntity
        {
            if (!IsOpen)
                OpenConnection();

            var context = new RepositoryContext<TEntity>(Connection.Connection);

            ObjectsToDispose.Add(context);

            return context;
        }
    }
}
