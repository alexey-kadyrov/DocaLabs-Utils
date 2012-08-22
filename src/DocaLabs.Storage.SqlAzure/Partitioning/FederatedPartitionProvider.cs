using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Utils;

namespace DocaLabs.Storage.SqlAzure.Partitioning
{
    /// <summary>
    /// Implements IPartitionConnectionProvider to be used with Sql Azure federations.
    /// </summary>
    public class FederatedPartitionProvider : IPartitionConnectionProvider
    {
        /// <summary>
        /// Gets or sets the default federation name which will be used when constructor with connection string only is used.
        /// </summary>
        public static string DefaultFederationName { get; set; }

        /// <summary>
        /// Gets or sets the default distribution name which will be used when constructor with connection string only is used.
        /// </summary>
        public static string DefaultDistributionName { get; set; }

        /// <summary>
        /// Gets connection string to the federation root.
        /// </summary>
        public DbConnectionString FederationRootConnectionString { get; private set; }

        /// <summary>
        /// Gets the federation name which the class instance is using.
        /// </summary>
        public string FederationName { get; private set; }


        /// <summary>
        /// Gets the distribution name which the class instance is using.
        /// </summary>
        public string DistributionName { get; private set; }

        static FederatedPartitionProvider()
        {
            DefaultFederationName = "FedName1";
            DefaultDistributionName = "FedDistr1";
        }

        /// <summary>
        /// Initializes an instance of the FederatedPartitionProvider class using default values for federation and distribution names.
        /// </summary>
        /// <param name="federationRootConnectionString">Connection string to a federation root.</param>
        public FederatedPartitionProvider(DbConnectionString federationRootConnectionString)
            : this(federationRootConnectionString, DefaultFederationName, DefaultDistributionName)
        {
        }

        /// <summary>
        /// Initializes an instance of the FederatedPartitionProvider class using provided values for federation and distribution names.
        /// </summary>
        /// <param name="federationRootConnectionString">Connection string to a federation root.</param>
        /// <param name="federationName">Federation name.</param>
        /// <param name="distributionName">Distribution name.</param>
        public FederatedPartitionProvider(DbConnectionString federationRootConnectionString, string federationName, string distributionName)
        {
            DistributionName = distributionName;
            FederationName = federationName;
            FederationRootConnectionString = federationRootConnectionString;
        }

        /// <summary>
        /// Returns a connection for a given partition key.
        /// </summary>
        /// <param name="partitionKey">The partition key which is used to get associated partition.</param>
        /// <returns>A new instance of a connection wrapper for the partition.</returns>
        /// <exception cref="PartitionException">If the partition's connection is not found.</exception>
        public IDbConnectionWrapper GetConnection(object partitionKey)
        {
            return new FederatedDbConnectionWrapper(FederationRootConnectionString, new FederationCommand(FederationName, DistributionName, partitionKey));
        }
    }
}
