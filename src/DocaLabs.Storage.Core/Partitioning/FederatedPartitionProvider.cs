namespace DocaLabs.Storage.Core.Partitioning
{
    /// <summary>
    /// Implements IPartitionConnectionProvider to be used with Sql Azure federations.
    /// </summary>
    public class FederatedPartitionProvider : IPartitionConnectionProvider
    {
        /// <summary>
        /// Gets connection string to the federation root.
        /// </summary>
        public DatabaseConnectionString FederationRootConnectionString { get; private set; }

        /// <summary>
        /// Gets the federation name which the class instance is using.
        /// </summary>
        public string FederationName { get; private set; }


        /// <summary>
        /// Gets the distribution name which the class instance is using.
        /// </summary>
        public string DistributionName { get; private set; }

        /// <summary>
        /// Initializes an instance of the FederatedPartitionProvider class using provided values for federation and distribution names.
        /// </summary>
        /// <param name="federationRootConnectionString">Connection string to a federation root.</param>
        /// <param name="federationName">Federation name.</param>
        /// <param name="distributionName">Distribution name.</param>
        public FederatedPartitionProvider(DatabaseConnectionString federationRootConnectionString, string federationName, string distributionName)
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
        public IDatabaseConnection GetConnection(object partitionKey)
        {
            return new FederatedDatabaseConnection(FederationRootConnectionString, new FederationCommand(FederationName, DistributionName, partitionKey));
        }
    }
}
