using System;
using System.ComponentModel;
using System.Configuration;

namespace DocaLabs.Storage.Core.Configuration
{
    /// <summary>
    /// Represents a configuration element that defines the map of entity type to table name. 
    /// </summary>
    public class EntityTableNameElement : ConfigurationElement
    {
        const string TypeProperty = "type";
        const string TableProperty = "table";

        /// <summary>
        /// Always returns false letting the element to be modified at runtime.
        /// </summary>
        /// <returns></returns>
        public override bool IsReadOnly()
        {
            return false;
        }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        [ConfigurationProperty(TypeProperty, IsKey = true, IsRequired = true), TypeConverter(typeof(TypeNameConverter))]
        public Type Type
        {
            get { return ((Type)base[TypeProperty]); }
            set { base[TypeProperty] = value;}
        }

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        [ConfigurationProperty(TableProperty, IsRequired = true)]
        public string Table
        {
            get { return (string)this[TableProperty]; }
            set { this[TableProperty] = value; }
        }
    }
}
