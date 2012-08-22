using DocaLabs.Testing.Common.MSpec;
using DocaLabs.Utils.Configuration;
using Machine.Specifications;
using Machine.Specifications.Annotations;
using Microsoft.WindowsAzure;
using Moq;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Azure.Integration.Tests
{
    class AzureStorageFactoryTestsContext
    {
        [UsedImplicitly] Establish before_each =
            () => CloudStorageAccount.SetConfigurationSettingPublisher(AzureStorageFactory.GetConfigurationSettingPublisher());

        [UsedImplicitly] Cleanup after_each = () =>
        {
            AzureStorageFactory.ConfigurationManager = null;
            AzureStorageFactory.DefaultConnectionString = null;
            AzureStorageFactory.UseDevelopmentStorageAccount = false;
            AzureStorageFactory.DefaultConnectionStringKey = "DataConnectionString";
        };
    }

    [Behaviors]
    class AzureStorageFactoryBehaviors
    {
        It should_create_cloud_storage_account =
            () => AzureStorageFactory.CreateAccount().ShouldNotBeNull();

        It should_create_cloud_blob_client =
            () => AzureStorageFactory.CreateCloudBlobClient().ShouldNotBeNull();

        It should_create_cloud_table_client =
            () => AzureStorageFactory.CreateCloudTableClient().ShouldNotBeNull();

        It should_create_cloud_queue_client =
            () => AzureStorageFactory.CreateCloudQueueClient().ShouldNotBeNull();

        It should_get_container_reference =
            () => AzureStorageFactory.GetContainerReference("docalabstestcontainer").ShouldNotBeNull();

        It should_get_blob_reference =
            () => AzureStorageFactory.GetBlobReference("docalabstestcontainer/blob").ShouldNotBeNull();

        It should_get_block_blob_refrence =
            () => AzureStorageFactory.GetBlockBlobReference("docalabstestcontainer/blockblob").ShouldNotBeNull();

        It should_get_page_blob_refrence =
            () => AzureStorageFactory.GetPageBlobReference("docalabstestcontainer/pageblob").ShouldNotBeNull();

        It should_get_table_service_context =
            () => AzureStorageFactory.GetTableServiceContext().ShouldNotBeNull();

        It should_get_queue_reference =
            () => AzureStorageFactory.GetQueueReference("docalabstestqueue").ShouldNotBeNull();
    }

    [Subject(typeof(AzureStorageFactory)), IntegrationTag]
    class when_azure_storage_factory_is_used_in_its_default_state : AzureStorageFactoryTestsContext
    {
        It default_connection_string_key_should_be_set_to_data_connection_string =
            () => AzureStorageFactory.DefaultConnectionStringKey.ShouldEqual("DataConnectionString");

        It default_connection_string_should_be_null =
            () => AzureStorageFactory.DefaultConnectionString.ShouldBeNull();

        It use_development_storage_account_should_be_false =
            () => AzureStorageFactory.UseDevelopmentStorageAccount.ShouldBeFalse();

        It configuration_manager_should_be_the_same_as_current_configuration_manager =
            () => AzureStorageFactory.ConfigurationManager.ShouldBeTheSameAs(CurrentConfigurationManager.Current);

        #pragma warning disable 0169
        Behaves_like<AzureStorageFactoryBehaviors> a_storage_factory;
        #pragma warning restore 0169
    }

    [Subject(typeof(AzureStorageFactory)), IntegrationTag]
    class when_azure_storage_factory_is_using_development_storgae_account : AzureStorageFactoryTestsContext
    {
        Because of = () =>
        {
            AzureStorageFactory.UseDevelopmentStorageAccount = true;
            AzureStorageFactory.DefaultConnectionStringKey = "Some unknown app setting key.";
            AzureStorageFactory.DefaultConnectionString = "Some badly formatted connection string.";
        };

        #pragma warning disable 0169
        Behaves_like<AzureStorageFactoryBehaviors> a_storage_factory;
        #pragma warning restore 0169
    }

    [Subject(typeof(AzureStorageFactory)), IntegrationTag]
    class when_azure_storage_factory_is_using_connection_string_directly : AzureStorageFactoryTestsContext
    {
        Because of = () =>
        {
            AzureStorageFactory.UseDevelopmentStorageAccount = false;
            AzureStorageFactory.DefaultConnectionStringKey = "Some unknown app setting key.";
            AzureStorageFactory.DefaultConnectionString = "UseDevelopmentStorage=true";
        };

        #pragma warning disable 0169
        Behaves_like<AzureStorageFactoryBehaviors> a_storage_factory;
        #pragma warning restore 0169
    }

    [Subject(typeof(AzureStorageFactory)), IntegrationTag]
    class when_changing_configuration_manager : AzureStorageFactoryTestsContext
    {
        static Mock<IConfigurationManager> mock_configuration_manager;

        Establish context =
            () => mock_configuration_manager = new Mock<IConfigurationManager>();

        Because of =
            () => AzureStorageFactory.ConfigurationManager = mock_configuration_manager.Object;

        It should_return_new_value =
            () => AzureStorageFactory.ConfigurationManager.ShouldBeTheSameAs(mock_configuration_manager.Object);
    }

    [Subject(typeof(AzureStorageFactory)), IntegrationTag]
    class when_changing_configuration_manager_and_then_setting_it_to_null : AzureStorageFactoryTestsContext
    {
        static Mock<IConfigurationManager> mock_configuration_manager;

        Establish context = () =>
        {
            mock_configuration_manager = new Mock<IConfigurationManager>();
            AzureStorageFactory.ConfigurationManager = mock_configuration_manager.Object;
        };

        Because of =
            () => AzureStorageFactory.ConfigurationManager = null;

        It configuration_manager_should_be_the_same_as_current_configuration_manager =
            () => AzureStorageFactory.ConfigurationManager.ShouldBeTheSameAs(CurrentConfigurationManager.Current);
    }
}
