using System.Configuration;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Testing.Common.MSpec;
using DocaLabs.Utils.Configuration;
using Machine.Specifications;
using Machine.Specifications.Annotations;
using Moq;
using It=Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Integration.Tests.Partitioning
{
    class DefaultPartitionProxyContext
    {
        protected static Mock<IConfigurationManager> configuration_manager;

        [UsedImplicitly] Cleanup after_each =
            () => CurrentConfigurationManager.Current = null;

        [UsedImplicitly] Establish before_each = () =>
        {
            configuration_manager = new Mock<IConfigurationManager>();
            configuration_manager.Setup(x => x.GetConnectionString(CurrentPartitionProxy.DefaultDataConnectionStringName))
                .Returns(new ConnectionStringSettings(CurrentPartitionProxy.DefaultDataConnectionStringName, "data-connection-string", "DummyProviderFactory"));

            CurrentConfigurationManager.Current = configuration_manager.Object;
        };
    }

    [Behaviors]
    class DefaultPartitionProxyBehaviors
    {
        It should_return_non_null_provider =
            () => CurrentPartitionProxy.Current.ShouldNotBeNull();

        It should_use_dummy_partiton_key_provider =
            () => CurrentPartitionProxy.Current.PartitionKeyProvider.ShouldBeOfType<DummyPartitionKeyProvider>();

        It should_use_single_partiton_provider =
            () => CurrentPartitionProxy.Current.PartitionConnectionProvider.ShouldBeOfType<SinglePartitionProvider>();
    }

    [Subject(typeof(CurrentPartitionProxy)), IntegrationTag]
    class when_current_partition_proxy_is_used_in_its_default_state : DefaultPartitionProxyContext
    {
        #pragma warning disable 0169
        Behaves_like<DefaultPartitionProxyBehaviors> a_default_partition_proxy;
        #pragma warning restore 0169
    }

    [Subject(typeof(CurrentPartitionProxy)), IntegrationTag]
    class when_setting_current_partition_proxy
    {
        static Mock<IPartitionProxy> partition_proxy;

        Cleanup after =
            () => CurrentPartitionProxy.Current = null;

        Establish context =
            () => partition_proxy = new Mock<IPartitionProxy>();

        Because of =
            () => CurrentPartitionProxy.Current = partition_proxy.Object;

        It should_return_specified_proxy =
            () => CurrentPartitionProxy.Current.ShouldBeTheSameAs(partition_proxy.Object);
    }

    [Subject(typeof(CurrentPartitionProxy)), IntegrationTag]
    class when_setting_current_partition_proxy_back_to_null : DefaultPartitionProxyContext
    {
        Cleanup after =
            () => CurrentPartitionProxy.Current = null;

        Establish context = 
            () => CurrentPartitionProxy.Current = new Mock<IPartitionProxy>().Object;

        Because of =
            () => CurrentPartitionProxy.Current = null;

        #pragma warning disable 0169
        Behaves_like<DefaultPartitionProxyBehaviors> a_default_partition_proxy;
        #pragma warning restore 0169
    }

    [Subject(typeof(CurrentPartitionProxy)), IntegrationTag]
    class when_creating_single_partition_provider
    {
        static Mock<IConfigurationManager> configuration_manager;
        static SinglePartitionProvider partition_provider;

        Cleanup after =
            () => CurrentConfigurationManager.Current = null;

        Establish context = () =>
        {
            configuration_manager = new Mock<IConfigurationManager>();
            configuration_manager.Setup(x => x.GetConnectionString(CurrentPartitionProxy.DefaultDataConnectionStringName))
                .Returns(new ConnectionStringSettings(CurrentPartitionProxy.DefaultDataConnectionStringName, "data-connection-string", "DummyProviderFactory"));

            CurrentConfigurationManager.Current = configuration_manager.Object;
        };

        Because of =
            () => partition_provider = CurrentPartitionProxy.CreateDefaultSinglePartitionProvider();

        It should_not_be_null =
            () => partition_provider.ShouldNotBeNull();
    }

    [Subject(typeof(CurrentPartitionProxy)), IntegrationTag]
    class when_creating_hashed_partition_provider
    {
        static Mock<IConfigurationManager> configuration_manager;
        static HashedPartitionProvider partition_provider;

        Cleanup after =
            () => CurrentConfigurationManager.Current = null;

        Establish context = () =>
        {
            configuration_manager = new Mock<IConfigurationManager>();
            configuration_manager.Setup(x => x.GetConnectionString(CurrentPartitionProxy.DefaultPartitionMapConnectionStringName))
                .Returns(new ConnectionStringSettings(CurrentPartitionProxy.DefaultPartitionMapConnectionStringName, "map-connection-string", "DummyProviderFactory"));

            CurrentConfigurationManager.Current = configuration_manager.Object;
        };

        Because of =
            () => partition_provider = CurrentPartitionProxy.CreateDefaultHashedPartitionProvider();

        It should_not_be_null =
            () => partition_provider.ShouldNotBeNull();
    }

    [Subject(typeof(CurrentPartitionProxy)), IntegrationTag]
    class when_creating_key_map_partition_provider
    {
        static Mock<IConfigurationManager> configuration_manager;
        static KeyMapPartitionProvider partition_provider;

        Cleanup after =
            () => CurrentConfigurationManager.Current = null;

        Establish context = () =>
        {
            configuration_manager = new Mock<IConfigurationManager>();
            configuration_manager.Setup(x => x.GetConnectionString(CurrentPartitionProxy.DefaultPartitionMapConnectionStringName))
                .Returns(new ConnectionStringSettings(CurrentPartitionProxy.DefaultPartitionMapConnectionStringName, "map-connection-string", "DummyProviderFactory"));

            CurrentConfigurationManager.Current = configuration_manager.Object;
        };

        Because of =
            () => partition_provider = CurrentPartitionProxy.CreateDefaultKeyMapPartitionProvider();

        It should_not_be_null =
            () => partition_provider.ShouldNotBeNull();
    }
}
