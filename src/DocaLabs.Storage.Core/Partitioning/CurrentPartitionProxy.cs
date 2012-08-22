using DocaLabs.Storage.Core.Utils;
using DocaLabs.Utils.Configuration;

namespace DocaLabs.Storage.Core.Partitioning
{
    /// <summary>
    /// Provides access to the current partition proxy.
    /// </summary>
    public static class CurrentPartitionProxy
    {
        /// <summary>
        /// The name of the connection string which is used be the default constructor.
        /// </summary>
        public const string DefaultPartitionMapConnectionStringName = "DefaultPartitionMapConnectionString";

        /// <summary>
        /// The name of the connection string which is used be the default constructor.
        /// </summary>
        public const string DefaultDataConnectionStringName = "DefaultDataConnectionString";

        static volatile IPartitionProxy _current;

        /// <summary>
        /// Gets/sets the current partition proxy. If the property is not set explicitly it will return partition proxy
        /// with dummy DummyPartitionKeyProvider and SinglePartitionProvider. If you are not defining the connection
        /// string with 'DefaultDataConnectionString' name in the config file this may cause the exception as the SinglePartitionProvider
        /// tries to get the connection string from the config file. You need to define the connection string in the config file
        /// or set your implementation before the getter is called.
        /// Setting the property to null will force to return the partition proxy with default settings next time the getter is called.
        /// The getter will never return null value.       
        /// </summary>
        public static IPartitionProxy Current
        {
            get { return _current ?? DefaultLazyPartitionProxy.LazyProxy; }
            set { _current = value; }
        }

        /// <summary>
        /// Initializes a new instance of the SinglePartitionProvider class using the connection string with the name 'DefaultDataConnectionString' for the data
        /// </summary>
        /// <returns>A new instance of the SinglePartitionProvider class.</returns>
        public static SinglePartitionProvider CreateDefaultSinglePartitionProvider()
        {
            return new SinglePartitionProvider(new DbConnectionString(
                CurrentConfigurationManager.Current.GetConnectionString(DefaultDataConnectionStringName)));
        }

        /// <summary>
        /// Initializes a new instance of the HashedPartitionProvider class using the connection string with the name 'DefaultPartitionMapConnectionString' to access partition information.
        /// </summary>
        /// <returns>A new instance of the HashedPartitionProvider class.</returns>
        public static HashedPartitionProvider CreateDefaultHashedPartitionProvider()
        {
            return new HashedPartitionProvider(new DbConnectionString(
                CurrentConfigurationManager.Current.GetConnectionString(DefaultPartitionMapConnectionStringName)));
        }

        /// <summary>
        /// Initializes a new instance of the KeyMapPartitionProvider class using the connection string with the name 'DefaultPartitionMapConnectionString' to access partition information.
        /// </summary>
        /// <returns>A new instance of the KeyMapPartitionProvider class.</returns>
        public static KeyMapPartitionProvider CreateDefaultKeyMapPartitionProvider()
        {
            return new KeyMapPartitionProvider(new DbConnectionString(
                CurrentConfigurationManager.Current.GetConnectionString(DefaultPartitionMapConnectionStringName)));
        }

        static class DefaultLazyPartitionProxy
        {
            internal static IPartitionProxy LazyProxy { get; private set; }

            static DefaultLazyPartitionProxy()
            {
                LazyProxy = new PartitionProxy(new DummyPartitionKeyProvider(), CreateDefaultSinglePartitionProvider());
            }
        }
    }
}
