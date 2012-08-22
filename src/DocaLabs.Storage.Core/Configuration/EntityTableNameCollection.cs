using System;
using System.Configuration;
using DocaLabs.Utils.Configuration;

namespace DocaLabs.Storage.Core.Configuration
{
    /// <summary>
    /// Contains a collection of EntityTableNameElement objects.
    /// </summary>
    public class EntityTableNameCollection : ConfigurationElementCollectionBase<Type, EntityTableNameElement>
    {
        /// <summary>
        /// Initializes an instance of the EntityTableNameCollection class.
        /// </summary>
        public EntityTableNameCollection()
            : base("entity")
        {
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EntityTableNameElement)element).Type;
        }
    }
}
