using DocaLabs.AzureStorage.Tables;
using DocaLabs.Storage.Core;
using Machine.Specifications;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Azure
{
    [Subject(typeof(TableExtensions))]
    class when_creating_tables_using_explicit_names_on_default_endpoints_for_cloud_storage_account : table_extensions_test_context
    {
        Cleanup after_each = () =>
        {
            DropTable("TableExtensionsTestsExplicitTable11");
            DropTable("TableExtensionsTestsExplicitTable12");
        };

        Because of =
            () => CloudStorageAccount.DevelopmentStorageAccount.CreateTableIfNotExist("TableExtensionsTestsExplicitTable11", "TableExtensionsTestsExplicitTable12");

        It should_create_first_table =
            () => IsTableExists("TableExtensionsTestsExplicitTable11").ShouldBeTrue();

        It should_create_second_table =
            () => IsTableExists("TableExtensionsTestsExplicitTable12").ShouldBeTrue();
    }

    [Subject(typeof(TableExtensions))]
    class when_creating_tables_using_explicit_names_for_table_client : table_extensions_test_context
    {
        Cleanup after_each = () =>
        {
            DropTable("TableExtensionsTestsExplicitTable11");
            DropTable("TableExtensionsTestsExplicitTable12");
        };

        Because of =
            () => CloudStorageAccount.DevelopmentStorageAccount.CreateCloudTableClient().CreateTableIfNotExist("TableExtensionsTestsExplicitTable11", "TableExtensionsTestsExplicitTable12");

        It should_create_first_table =
            () => IsTableExists("TableExtensionsTestsExplicitTable11").ShouldBeTrue();

        It should_create_second_table =
            () => IsTableExists("TableExtensionsTestsExplicitTable12").ShouldBeTrue();
    }

    [Subject(typeof(TableExtensions))]
    class when_creating_tables_using_name_resolution_on_default_endpoints_for_cloud_storage_account : table_extensions_test_context
    {
        Cleanup after_each = () =>
        {
            DropTable(EntityToNameMap.Get<AzureRepositoryTest21Product>());
            DropTable(EntityToNameMap.Get<AzureRepositoryTest22Product>());
        };

        Because of =
            () => CloudStorageAccount.DevelopmentStorageAccount.CreateTableIfNotExist(typeof(AzureRepositoryTest21Product), typeof(AzureRepositoryTest22Product));

        It should_create_first_table =
            () => IsTableExists(EntityToNameMap.Get<AzureRepositoryTest21Product>()).ShouldBeTrue();

        It should_create_second_table =
            () => IsTableExists(EntityToNameMap.Get<AzureRepositoryTest22Product>()).ShouldBeTrue();

        class AzureRepositoryTest21Product : TableEntity
        {
        }

        class AzureRepositoryTest22Product : TableEntity
        {
        }
    }

    [Subject(typeof(TableExtensions))]
    class when_creating_tables_using_name_resolution_for_table_client : table_extensions_test_context
    {
        Cleanup after_each = () =>
        {
            DropTable(EntityToNameMap.Get<AzureRepositoryTest21Product>());
            DropTable(EntityToNameMap.Get<AzureRepositoryTest22Product>());
        };

        Because of =
            () => CloudStorageAccount.DevelopmentStorageAccount.CreateCloudTableClient().CreateTableIfNotExist(typeof(AzureRepositoryTest21Product), typeof(AzureRepositoryTest22Product));

        It should_create_first_table =
            () => IsTableExists(EntityToNameMap.Get<AzureRepositoryTest21Product>()).ShouldBeTrue();

        It should_create_second_table =
            () => IsTableExists(EntityToNameMap.Get<AzureRepositoryTest22Product>()).ShouldBeTrue();

        class AzureRepositoryTest21Product : TableEntity
        {
        }

        class AzureRepositoryTest22Product : TableEntity
        {
        }
    }

    class table_extensions_test_context
    {
        protected static bool IsTableExists(string tableName)
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            return table.Exists();
        }

        protected static void DropTable(string tableName)
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            table.DeleteIfExists();
        }
    }
}
