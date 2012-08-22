using System.Configuration;

namespace DocaLabs.Utils.Configuration
{
    /// <summary>
    /// Provides access to configuration files for client applications.
    /// </summary>
    public class DefaultConfigurationManager : IConfigurationManager
    {
        /// <summary>
        /// Gets the appSettings value for the current configuration.
        /// </summary>
        /// <param name="name">The key of the entry to locate.</param>
        /// <returns>The string that contains the value associated with the specified key, if found; otherwise, null.</returns>
        public string GetAppSetting(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        /// <summary>
        /// Gets the connectionStrings value for the current configuration.
        /// </summary>
        /// <param name="name">The key of the entry to locate.</param>
        /// <returns>The ConnectionStringSettings object with the specified name; otherwise, null.</returns>
        public ConnectionStringSettings GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name];
        }

        /// <summary>
        /// Retrieves a specified strongly typed configuration section for the current configuration.
        /// </summary>
        /// <typeparam name="T">The type of the configuration section.</typeparam>
        /// <param name="sectionName">The configuration section path and name.</param>
        /// <returns>The specified ConfigurationSection object, or null if the section does not exist or of type T.</returns>
        public T GetSection<T>(string sectionName) where T : ConfigurationSection
        {
            return ConfigurationManager.GetSection(sectionName) as T;
        }

        /// <summary>
        /// Retrieves a specified configuration section for the current configuration.
        /// </summary>
        /// <param name="sectionName">The configuration section path and name.</param>
        /// <returns>The specified ConfigurationSection object, or null if the section does not exist.</returns>
        public object GetSection(string sectionName)
        {
            return ConfigurationManager.GetSection(sectionName);
        }
    }
}
