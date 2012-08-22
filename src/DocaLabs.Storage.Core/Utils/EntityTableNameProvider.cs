using System;
using System.Collections.Concurrent;
using DocaLabs.Storage.Core.Configuration;

namespace DocaLabs.Storage.Core.Utils
{
    /// <summary>
    /// Resolves a type into table name, the resolution uses next sequence:
    ///     1. Fires Event and if somebody resolves the name then accept it
    ///     2. Tries to get from the configuration file if it's defined
    ///     3. Tries to get from TableNameAttribute if it's defined on the entity type
    ///     4. Tries to get from System.Data.Linq.Mapping.TableAttribute if it's defined on the entity type
    ///     5. Gets the type name (pluralise the name using Inflector)
    ///     6. Fires Event to give ability to modify the resolved name if needed
    /// The result of resolving is cached.
    /// </summary>
    public class EntityTableNameProvider : IEntityTableNameProvider
    {
        ConcurrentDictionary<Type, string> TypeToNameMap { get; set; }

        /// <summary>
        /// The event is fired when the type is being resolved before any other actions in the pipeline.
        /// By setting the TableName to any non whitespace string will be accept as the table name.
        /// The event is fired only when the type is resolved for the first time as after resolving the name will be put into the cache.
        /// </summary>
        public event EventHandler<EntityTableNameArgs> ResolvingEntityToTableName;

        /// <summary>
        /// The event is fired when the type is being resolved after any other actions in the pipeline.
        /// The table name cam be modified at this point.
        /// The event is fired only when the type is resolved for the first time as after resolving the name will be put into the cache.
        /// </summary>
        public event EventHandler<EntityTableNameArgs> ResolvedEntityToTableName;

        /// <summary>
        /// Initializes an instance of the EntityTableNameProvider class.
        /// </summary>
        public EntityTableNameProvider()
        {
            TypeToNameMap = new ConcurrentDictionary<Type, string>();
        }

        /// <summary>
        /// Resolves the type to a table name, the resolution uses next sequence:
        ///     1. Fires Event and if somebody resolves the name then accept it
        ///     2. Tries to get from the configuration file if it's defined
        ///     3. Tries to get from TableNameAttribute if it's defined on the entity type
        ///     4. Tries to get from System.Data.Linq.Mapping.TableAttribute if it's defined on the entity type
        ///     5. Gets the type name (pluralise the name using Inflector)
        ///     6. Fires Event to give ability to modify the resolved name if needed
        /// </summary>
        /// <typeparam name="TEntity">Type to be resolved.</typeparam>
        /// <returns>Table name.</returns>
        public string Resolve<TEntity>()
        {
            return Resolve(typeof(TEntity));
        }

        /// <summary>
        /// Resolves the type to a table name, the resolution uses next sequence:
        ///     1. Fires Event and if somebody resolves the name then accept it
        ///     2. Tries to get from the configuration file if it's defined
        ///     3. Tries to get from TableNameAttribute if it's defined on the entity type
        ///     4. Tries to get from System.Data.Linq.Mapping.TableAttribute if it's defined on the entity type
        ///     5. Gets the type name (pluralise the name using Inflector)
        ///     6. Fires Event to give ability to modify the resolved name if needed
        /// </summary>
        /// <param name="entityType">Type to be resolved.</param>
        /// <returns>Table name.</returns>
        public string Resolve(Type entityType)
        {
            if(entityType == null)
                throw new ArgumentNullException("entityType");

            return TypeToNameMap.GetOrAdd(entityType, ExtractTableName);
        }

        string ExtractTableName(Type entityType)
        {
            var eventArgs = new EntityTableNameArgs(entityType);

            OnResolvingEntityToTableName(eventArgs);

            var tableName = eventArgs.TableName;

            if (string.IsNullOrWhiteSpace(eventArgs.TableName))
                tableName = new TableNameExtractor(entityType).Extract();

            OnResolvedEntityToTableName(eventArgs);

            return !string.IsNullOrWhiteSpace(eventArgs.TableName) ? eventArgs.TableName : tableName;
        }

        void OnResolvingEntityToTableName(EntityTableNameArgs args)
        {
            if (ResolvingEntityToTableName != null)
                ResolvingEntityToTableName(this, args);
        }

        void OnResolvedEntityToTableName(EntityTableNameArgs args)
        {
            if (ResolvedEntityToTableName != null)
                ResolvedEntityToTableName(this, args);
        }
    
        class TableNameExtractor
        {
            Type EntityType { get; set; }

            public TableNameExtractor(Type entityType)
            {
                EntityType = entityType;

            }

            public string Extract()
            {
                return TryConfigurationFile() ?? TryTableNameAttribute() ?? TryTableAttribute() ?? Inflector.Inflector.Pluralize(EntityType.Name);
            }

            string TryConfigurationFile()
            {
                var section = EntityTableNameSection.GetDefaultSection();
                if (section == null)
                    return null;

                var configElement = section.Map[EntityType];

                return configElement != null && !string.IsNullOrWhiteSpace(configElement.Table)
                    ? configElement.Table
                    : null;
            }

            string TryTableNameAttribute()
            {
                var attribute = GetTableAttribute<TableNameAttribute>(EntityType);

                return attribute != null && (!string.IsNullOrWhiteSpace(attribute.Name))
                    ? attribute.Name
                    : null;
            }

            string TryTableAttribute()
            {
                var tableNameAttribute3 = GetTableAttribute<System.Data.Linq.Mapping.TableAttribute>(EntityType);

                return tableNameAttribute3 != null && (!string.IsNullOrWhiteSpace(tableNameAttribute3.Name))
                    ? tableNameAttribute3.Name
                    : null;
            }

            static T GetTableAttribute<T>(Type info) where T : class
            {
                var attributes = info.GetCustomAttributes(typeof(T), true);

                return attributes.Length > 0 ? attributes[0] as T : null;
            }
        }
    }
}
