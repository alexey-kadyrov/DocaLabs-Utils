using System;
using System.Linq;
using DocaLabs.Storage.Core;
using NHibernate;

namespace DocaLabs.Storage.NHibernate
{
    /// <summary>
    /// Defines methods for the runtime context of the NHibernate data service.
    /// </summary>
    public interface ISessionContext : IStorageUnit, IDisposable
    {
        /// <summary>
        /// Provides access to NHibernate's runtime session.
        /// </summary>
        ISession Session { get; }

        /// <summary>
        /// Creates a Linq query for data of a specified generic type.
        /// </summary>
        /// <typeparam name="T">The type returned by the query.</typeparam>
        /// <returns>A new IQueryable(Of TElement) instance that represents a query.</returns>
        IQueryable<T> CreateQuery<T>();

        /// <summary>
        /// Removes a persistent instance from the datastore.
        /// </summary>
        /// <remarks>
        /// The argument may be an instance associated with the receiving <c>ISession</c> or a
        /// transient instance with an identifier associated with existing persistent state.
        /// </remarks>
        /// <param name="obj">The instance to be removed</param>
        void Remove(object obj);

        /// <summary>
        /// Returns the persistent instance of the given entity class with the given identifier, or null
        /// if there is no such persistent instance. (If the instance, or a proxy for the instance, is
        /// already associated with the session, return that instance or proxy.)
        /// </summary>
        /// <typeparam name="T">a persistent class type.</typeparam>
        /// <param name="id">an identifier.</param>
        /// <returns>a persistent instance or null</returns>
        T Get<T>(object id);

        /// <summary>
        /// Persists the given transient instance, first assigning a generated identifier.
        /// </summary>
        /// <remarks>
        /// Add will use the current value of the identifier property if the <c>Assigned</c>
        /// generator is used.
        /// </remarks>
        /// <param name="obj">A transient instance of a persistent class</param>
        /// <returns>The generated identifier</returns>
        object Add(object obj);

        /// <summary>
        /// Rereads the state of the given instance from the underlying database.
        /// </summary>
        void Refresh(object obj);
    }
}
