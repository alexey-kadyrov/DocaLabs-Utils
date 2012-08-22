using System;

namespace DocaLabs.Storage.Core.Utils
{
    /// <summary>
    /// Provides data for the ResolvingEntityToTableName and ResolvedEntityToTableName events of the EntityTableNameProvider class.
    /// </summary>
    public class EntityTableNameArgs : EventArgs
    {
        /// <summary>
        /// Gets the entity type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets or set the table name. The initial value is null. 
        /// If some consumer sets this property to non whitespace string
        /// it'll be used as the table name for the type.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Initializes a new instance of the EntityTableNameArgs class with specified entity type.
        /// </summary>
        /// <param name="type">The entity type.</param>
        public EntityTableNameArgs(Type type)
        {
            Type = type;
        }
    }
}
