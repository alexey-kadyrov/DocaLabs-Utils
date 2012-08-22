using System;
using System.Configuration;
using DocaLabs.Utils.Configuration;

namespace DocaLabs.Storage.Core.Configuration
{
    /// <summary>
    /// Represents the section of a configuration file, which defines a map of entity types to table names. 
    /// </summary>
    public class EntityTableNameSection : ConfigurationSection
    {
        /// <summary>
        /// The default section name.
        /// </summary>
        public const string DefaultSectionName = "entityTableNameMap";

        /// <summary>
        /// Always returns false letting the section to be modified at runtime.
        /// </summary>
        /// <returns></returns>
        public override bool IsReadOnly()
        {
            return false;
        }

        /// <summary>
        /// Gets or sets a map of entity type to table name.
        /// </summary>
        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public EntityTableNameCollection Map
        {
            get { return (EntityTableNameCollection)this[""]; }
        }

        /// <summary>
        /// Retrieves a EntityTableNameSection configuration section using the default name.
        /// </summary>
        /// <returns>The specified ConfigurationSection object, or null if the section does not exist.</returns>
        public static EntityTableNameSection GetDefaultSection()
        {
            return GetSection(DefaultSectionName);
        }

        /// <summary>
        ///  Retrieves a specified EntityTableNameSection configuration section.
        /// </summary>
        /// <param name="sectionName">Section name.</param>
        /// <returns>The specified ConfigurationSection object, or null if the section does not exist.</returns>
        public static EntityTableNameSection GetSection(string sectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
                throw new ArgumentNullException("sectionName");

            return CurrentConfigurationManager.Current.GetSection(sectionName) as EntityTableNameSection;
        }
    }
}
