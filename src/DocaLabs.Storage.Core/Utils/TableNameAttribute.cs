using System;

namespace DocaLabs.Storage.Core.Utils
{
    /// <summary>
    /// Designates a class as an entity class that is associated with a table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TableNameAttribute class with specified table name.
        /// </summary>
        /// <param name="name">Table name.</param>
        public TableNameAttribute(string name)
        {
            Name = name;
        }
    }
}
