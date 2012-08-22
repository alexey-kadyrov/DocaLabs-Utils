using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace DocaLabs.Storage.NHibernate
{
    /// <summary>
    /// Implements the ISessionContext for the NHibernate data service.
    /// </summary>
    public class SessionContext : ISessionContext
    {
        /// <summary>
        /// Provides access to NHibernate's runtime session.
        /// </summary>
        public ISession Session { get; private set; }

        /// <summary>
        /// Initializes an instance of the SessionContext for specified session factory.
        /// </summary>
        public SessionContext(ISession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            Session = session;
        }

        /// <summary>
        /// Saves the changes being tracked to storage.
        /// </summary>
        public void SaveChanges()
        {
            Session.Flush();
        }

        /// <summary>
        /// Rereads the state of the given instance from the underlying database
        /// </summary>
        public void Refresh(object obj)
        {
            Session.Refresh(obj);
        }

        /// <summary>
        /// Creates a Linq query for data of a specified generic type.
        /// </summary>
        /// <typeparam name="T">The type returned by the query.</typeparam>
        /// <returns>A new IQueryable(Of TElement) instance that represents a query.</returns>
        public IQueryable<T> CreateQuery<T>()
        {
            return Session.Query<T>();
        }

        /// <summary>
        /// Removes a persistent instance from the datastore.
        /// </summary>
        /// <remarks>
        /// The argument may be an instance associated with the receiving <c>ISession</c> or a
        /// transient instance with an identifier associated with existing persistent state.
        /// </remarks>
        /// <param name="obj">The instance to be removed</param>
        public void Remove(object obj)
        {
            Session.Delete(obj);
        }

        /// <summary>
        /// Returns the persistent instance of the given entity class with the given identifier, or null
        /// if there is no such persistent instance. (If the instance, or a proxy for the instance, is
        /// already associated with the session, return that instance or proxy.)
        /// </summary>
        /// <typeparam name="T">a persistent class type.</typeparam>
        /// <param name="id">an identifier.</param>
        /// <returns>a persistent instance or null</returns>
        public T Get<T>(object id)
        {
            return Session.Get<T>(id);
        }

        /// <summary>
        /// Persists the given transient instance, first assigning a generated identifier.
        /// </summary>
        /// <remarks>
        /// Add will use the current value of the identifier property if the <c>Assigned</c>
        /// generator is used.
        /// </remarks>
        /// <param name="obj">A transient instance of a persistent class</param>
        /// <returns>The generated identifier</returns>
        public object Add(object obj)
        {
            return Session.Save(obj);
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
            if(disposing && Session != null)
                Session.Dispose();
        }
    }
}
