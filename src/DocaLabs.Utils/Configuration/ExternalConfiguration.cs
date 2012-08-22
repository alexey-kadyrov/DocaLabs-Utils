using System;
using System.Configuration;
using System.IO;
using DocaLabs.Utils.Resources;

namespace DocaLabs.Utils.Configuration
{
    /// <summary>
    /// Provides access to arbitrary configuration files.
    /// </summary>
    public class ExternalConfiguration : IConfigurationManager
    {
        /// <summary>
        /// Gets the underlying System.Configuration.Configuration object
        /// </summary>
        public System.Configuration.Configuration CurrentConfiguration { get; private set; }

        /// <summary>
        /// Loads the arbitrary configuration file and replaces the Current.
        /// </summary>
        /// <param name="fileName">The name of the configuration file.</param>
        /// <returns>The ExternalConfiguration object.</returns>
        public static IConfigurationManager ReplaceCurrentBy(string fileName)
        {
            var config = new ExternalConfiguration(fileName);

            CurrentConfigurationManager.Current = config;

            return config;
        }

        /// <summary>
        /// Initializes a new instance of the ExternalConfiguration class.
        /// </summary>
        /// <param name="fileName">The name of the configuration file.</param>
        public ExternalConfiguration(string fileName)
        {
            if(!File.Exists(fileName))
                throw new FileNotFoundException(String.Format(Text.configuration_file_0_not_found, fileName), fileName);

            CurrentConfiguration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
            {
                ExeConfigFilename = fileName
            }, ConfigurationUserLevel.None);
        }

        /// <summary>
        /// Gets the appSettings value for the current configuration.
        /// </summary>
        /// <param name="name">The key of the entry to locate.</param>
        /// <returns>The string that contains the value associated with the specified key, if found; otherwise, null.</returns>
        public string GetAppSetting(string name)
        {
            var s = CurrentConfiguration.AppSettings.Settings[name];

            return s != null ? s.Value : null;
        }

        /// <summary>
        /// Gets the connectionStrings value for the current configuration.
        /// </summary>
        /// <param name="name">The key of the entry to locate.</param>
        /// <returns>The ConnectionStringSettings object with the specified name; otherwise, null.</returns>
        public ConnectionStringSettings GetConnectionString(string name)
        {
            return CurrentConfiguration.ConnectionStrings.ConnectionStrings[name];
        }

        /// <summary>
        /// Retrieves a specified strongly typed configuration section for the current configuration.
        /// </summary>
        /// <typeparam name="T">The type of the configuration section.</typeparam>
        /// <param name="sectionName">The configuration section path and name.</param>
        /// <returns>The specified ConfigurationSection object, or null if the section does not exist or of type T.</returns>
        public T GetSection<T>(string sectionName) where T : ConfigurationSection
        {
            return CurrentConfiguration.GetSection(sectionName) as T;
        }

        /// <summary>
        /// Retrieves a specified configuration section for the current configuration.
        /// </summary>
        /// <param name="sectionName">The configuration section path and name.</param>
        /// <returns>The specified ConfigurationSection object, or null if the section does not exist.</returns>
        public object GetSection(string sectionName)
        {
            return CurrentConfiguration.GetSection(sectionName);
        }
    }
}
