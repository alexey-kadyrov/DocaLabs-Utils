using System;

namespace DocaLabs.Storage.Core.Utils
{
    /// <summary>
    /// Defines method to resolves a type into table name.
    /// </summary>
    public interface IEntityTableNameProvider
    {
        /// <summary>
        /// Resolves the type to a table name.
        /// </summary>
        /// <typeparam name="TEntity">Type to be resolved.</typeparam>
        /// <returns>Table name.</returns>
        string Resolve<TEntity>();

        /// <summary>
        /// Resolves the type to a table name.
        /// </summary>
        /// <param name="entityType">Type to be resolved.</param>
        /// <returns>Table name.</returns>
        string Resolve(Type entityType);
    }
}
