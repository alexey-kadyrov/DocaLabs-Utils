using System;
using System.Data.Services.Client;
using DocaLabs.Utils.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace DocaLabs.Storage.Azure
{
    /// <summary>
    /// Provides helper methods to create Windows Azure storage objects.
    /// </summary>
    public static class AzureStorageFactory
    {
        static volatile IConfigurationManager _configurationManager;

        /// <summary>
        /// Gets or sets the default connection string key in the role's configuration.
        /// The default value is "DataConnectionString".
        /// </summary>
        public static string DefaultConnectionStringKey { get; set; }

        /// <summary>
        /// Gets or sets the default connection string
        /// </summary>
        public static string DefaultConnectionString { get; set; }

        /// <summary>
        /// Gets or sets whenever to use the development storage. This is intended to be used in unit tests.
        /// In order to use the development storage in other scenarios better to set DefaultConnectionString to "UseDevelopmentStorage=true"
        /// or set the value of the DefaultConnectionStringKey to "UseDevelopmentStorage=true".
        /// </summary>
        public static bool UseDevelopmentStorageAccount { get; set; }

        /// <summary>
        /// Gets or sets current IConfigurationManager.
        /// Setting the property to null will force to return the CurrentConfigurationManager.Current next time the getter is called.
        /// The getter will never return null value.
        /// </summary>
        public static IConfigurationManager ConfigurationManager
        {
            get { return _configurationManager ?? CurrentConfigurationManager.Current; }
            set { _configurationManager = value; }
        }

        static AzureStorageFactory()
        {
            DefaultConnectionStringKey = "DataConnectionString";
        }

        /// <summary>
        /// Creates an instance of the CloudStorageAccount class.
        /// Checks the parameters in the next order:
        ///     1. if UseDevelopmentStorageAccount is set to true
        ///     2. if DefaultConnectionString is not null or white string
        ///     3. Use DefaultConnectionStringKey
        /// </summary>
        /// <returns></returns>
        public static CloudStorageAccount CreateAccount()
        {
            if (UseDevelopmentStorageAccount)
                return CloudStorageAccount.DevelopmentStorageAccount;

            return !string.IsNullOrWhiteSpace(DefaultConnectionString)
                ? CloudStorageAccount.Parse(DefaultConnectionString)
                : CloudStorageAccount.FromConfigurationSetting(DefaultConnectionStringKey);
        }

        /// <summary>
        /// Gets the configuration setting publisher for the storage account, to be used with CloudStorageAccount.SetConfigurationSettingPublisher.
        /// </summary>
        /// <returns>The configuration setting publisher for the storage account.</returns>
        public static Action<string, Func<string, bool>> GetConfigurationSettingPublisher()
        {
            if (RoleEnvironment.IsAvailable)
                return (configName, configSetter) => configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));

            return (configName, configSetter) => configSetter(ConfigurationManager.GetAppSetting(configName));
        }

        /// <summary>
        /// Safely gets the role settings, if it's not in the Azure environment then it'll look at app settings.
        /// </summary>
        /// <param name="configurationSettingName">The name of the configuration setting.</param>
        /// <returns>A String that contains the value of the configuration setting.</returns>
        public static string GetConfigurationSettingValue(string configurationSettingName)
        {
            return RoleEnvironment.IsAvailable ? 
                RoleEnvironment.GetConfigurationSettingValue(configurationSettingName) : 
                ConfigurationManager.GetAppSetting(configurationSettingName);
        }

        /// <summary>
        /// Creates a new Blob service client with the default end point and credentials.
        /// </summary>
        /// <returns>A client object that specifies the Blob service endpoint.</returns>
        public static CloudBlobClient CreateCloudBlobClient()
        {
            return CreateAccount().CreateCloudBlobClient();
        }

        /// <summary>
        /// Creates the Table service client with the default end point and credentials.
        /// </summary>
        /// <returns>A client object that specifies the Table service endpoint.</returns>
        public static CloudTableClient CreateCloudTableClient()
        {
            return CreateAccount().CreateCloudTableClient();
        }

        /// <summary>
        /// Creates a new Queue service client with the default end point and credentials.
        /// </summary>
        /// <returns>A client object that specifies the Queue service endpoint.</returns>
        public static CloudQueueClient CreateCloudQueueClient()
        {
            return CreateAccount().CreateCloudQueueClient();
        }

        /// <summary>
        /// Returns a reference to a CloudBlobContainer object with the specified address and default end point and credentials.
        /// </summary>
        /// <param name="containerAddress">The name of the container, or an absolute URI to the container.</param>
        /// <returns>A reference to a container.</returns>
        public static CloudBlobContainer GetContainerReference(string containerAddress)
        {
            return CreateAccount().CreateCloudBlobClient().GetContainerReference(containerAddress);
        }

        /// <summary>
        /// Returns a reference to a CloudBlob with the specified address and default end point and credentials. 
        /// </summary>
        /// <param name="blobAddress">The absolute URI to the blob, or a relative URI beginning with the container name.</param>
        /// <returns>A reference to a blob.</returns>
        public static CloudBlob GetBlobReference(string blobAddress)
        {
            return CreateAccount().CreateCloudBlobClient().GetBlobReference(blobAddress);
        }

        /// <summary>
        /// Returns a reference to a CloudBlockBlob with the specified address and default end point and credentials.
        /// </summary>
        /// <param name="blobAddress">The absolute URI to the blob, or a relative URI beginning with the container name.</param>
        /// <returns>A reference to a block blob.</returns>
        public static CloudBlockBlob GetBlockBlobReference(string blobAddress)
        {
            return CreateAccount().CreateCloudBlobClient().GetBlockBlobReference(blobAddress);
        }

        /// <summary>
        /// Returns a reference to a CloudPageBlob object with the specified address and default end point and credentials.
        /// </summary>
        /// <param name="blobAddress">The absolute URI to the blob, or a relative URI beginning with the container name.</param>
        /// <returns>A reference to a page blob.</returns>
        public static CloudPageBlob GetPageBlobReference(string blobAddress)
        {
            return CreateAccount().CreateCloudBlobClient().GetPageBlobReference(blobAddress);
        }

        /// <summary>
        /// Returns a reference to a TableServiceContext object with the specified address and default end point and credentials.
        /// The IgnoreResourceNotFoundException property is set to true and SaveChangesDefaultOptions is set to SaveChangesOptions.ReplaceOnUpdate
        /// </summary>
        /// <returns>A TableServiceContext object.</returns>
        public static TableServiceContext GetTableServiceContext()
        {
            var account = CreateAccount();

            return new TableServiceContext(account.TableEndpoint.ToString(), account.Credentials)
            {
                IgnoreResourceNotFoundException = true,
                SaveChangesDefaultOptions = SaveChangesOptions.ReplaceOnUpdate
            };
        }

        /// <summary>
        /// Gets a reference to the queue at the specified address and default end point and credentials.
        /// </summary>
        /// <param name="queueAddress">Either the name of the queue, or the absolute URI to the queue.</param>
        /// <returns>A reference to the queue.</returns>
        public static CloudQueue GetQueueReference(string queueAddress)
        {
            return CreateAccount().CreateCloudQueueClient().GetQueueReference(queueAddress);
        }
    }
}
