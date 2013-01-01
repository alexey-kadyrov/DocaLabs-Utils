using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocaLabs.Storage.Core
{
    /// <summary>
    /// Resolves the type to a table name, the resolution uses next sequence:
    ///     1. Tries to get from dictionary of already resolved/configured entity types
    ///     2. Tries to get from System.ComponentModel.DataAnnotations.Schema.TableAttribute if it's defined on the entity type
    ///     3. Gets the type name (pluralise the name using Inflector)
    /// The result of the resolving is cached.
    /// </summary>
    public static class EntityToNameMap
    {
        static readonly ConcurrentDictionary<Type, string> Map = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Adds a name for an entity type.
        /// </summary>
        /// <typeparam name="TEntity">Type which name should be stored.</typeparam>
        /// <param name="name">Table name.</param>
        public static void Configure<TEntity>(string name)
        {
            Configure(typeof(TEntity), name);
        }

        /// <summary>
        /// Adds a name for an entity type.
        /// </summary>
        /// <param name="entityType">Type which name should be stored.</param>
        /// <param name="name">Table name.</param>
        public static void Configure(Type entityType, string name)
        {
            Map.AddOrUpdate(entityType, name, (k, v) => name);
        }

        /// <summary>
        /// Resolves the type to a table name, the resolution uses next sequence:
        ///     1. Tries to get from dictionary of already resolved/configured entity types
        ///     2. Tries to get from System.ComponentModel.DataAnnotations.Schema.TableAttribute if it's defined on the entity type
        ///     3. Gets the type name (pluralise the name using Inflector)
        /// </summary>
        /// <typeparam name="TEntity">Type to be resolved.</typeparam>
        /// <returns>Table name.</returns>
        public static string Get<TEntity>()
        {
            return Get(typeof(TEntity));
        }

        /// <summary>
        /// Resolves the type to a table name, the resolution uses next sequence:
        ///     1. Tries to get from dictionary of already resolved/configured entity types
        ///     2. Tries to get from System.ComponentModel.DataAnnotations.Schema.TableAttribute if it's defined on the entity type
        ///     3. Gets the type name (pluralise the name using Inflector)
        /// </summary>
        /// <param name="entityType">Type to be resolved.</param>
        /// <returns>Table name.</returns>
        public static string Get(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");

            return Map.GetOrAdd(entityType, Resolve);
        }

        static string Resolve(Type entityType)
        {
            return TryTableNameAttribute(entityType) 
                ?? Inflector.Inflector.Pluralize(entityType.Name);
        }

        static string TryTableNameAttribute(Type entityType)
        {
            var attribute = GetTableAttribute<TableAttribute>(entityType);

            return attribute != null && (!string.IsNullOrWhiteSpace(attribute.Name))
                ? attribute.Name
                : null;
        }

        static T GetTableAttribute<T>(Type info) where T : class
        {
            var attributes = info.GetCustomAttributes(typeof(T), true);

            return attributes.Length > 0 
                ? attributes[0] as T 
                : null;
        }
    }
}
