using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Storage.EntityFramework;
using DocaLabs.Storage.SqlAzure.Integration.Tests.Entities;
using DocaLabs.Storage.SqlAzure.Partitioning;
using DocaLabs.Testing.Common.Database;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.SqlAzure.Integration.Tests
{
    [Subject("Federation tests for Type Framework Repository")]
    class when_entity_freamework_repository_is_used_with_federated_database
    {
        const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsFederatedRepositoryTests;Integrated Security=SSPI;";

        static Mock<IPartitionKeyProvider> partition_key_provider;
        static long customer_id;
        static IDbConnectionManager session_manager;
        static IRepository<Customer> customers;
        static Customer customer;
        static string original_federtaion_statemnet;

        Cleanup after_each = () =>
        {
            CurrentPartitionProxy.Current = null;
            FederationCommand.FederationStatement = original_federtaion_statemnet;
            session_manager.Dispose();
            MsSqlDatabaseBuilder.Drop(ConnectionString);
        };

        Establish context = () =>
        {
            MsSqlDatabaseBuilder.Build(ConnectionString, @"create-federation.sql");

            RepositoryConfiguration.RemoveInitializer<Customer>();

            customer_id = 29660;

            // the test simulates the federation switch, it's possible to test against real federation: you will need 
            //  * to uncomment corresponding statements in create-federation.sql
            //  * comment out next two lines
            //  * change the expected FirstName and LastName below
            //  * provide real connection string to Sql Azure instance
            original_federtaion_statemnet = FederationCommand.FederationStatement;
            FederationCommand.FederationStatement = "UPDATE [dbo].[Customers] SET [FirstName] = '{0}', [LastName]= '{1}' WHERE [CustomerId] = {2}";

            partition_key_provider = new Mock<IPartitionKeyProvider>();
            partition_key_provider.Setup(x => x.GetPartitionKey()).Returns(customer_id);

            CurrentPartitionProxy.Current = new PartitionProxy(
                partition_key_provider.Object,
                new FederatedPartitionProvider(new DbConnectionString(ConnectionString), "CustomerFederation", "cid"));

            session_manager = new PartitionedDbConnectionManager();

            customers = new Repository<Customer>(session_manager);
        };

        Because of =
            () => customer = customers.Find(customer_id);

        It should_fetch_expected_customer = () => customer.ShouldBeSimilar(new Customer
        {
            CustomerId = customer_id,
            Title = "Mr.",
            FirstName = "CustomerFederation", //"Anthony",
            MiddleName = null,
            LastName = "cid", // "Chor",
            Suffix = null,
            CompanyName = "Extreme Riding Supplies",
            SalesPerson = @"adventure-works\linda3",
            EmailAddress = "anthony0@adventure-works.com",
            Phone = "429-555-0145"
        });
    }
}
