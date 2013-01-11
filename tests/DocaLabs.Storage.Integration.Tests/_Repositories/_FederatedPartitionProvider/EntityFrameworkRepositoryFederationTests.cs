using System.Data.Entity;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Repositories;
using DocaLabs.EntityFrameworkStorage;
using DocaLabs.Testing.Common;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Integration.Tests._Repositories._FederatedPartitionProvider
{
    [Subject("Federation tests for Type Framework Repository")]
    class when_entity_freamework_repository_is_used_with_federated_database
    {
        const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsFederatedRepositoryTests;Integrated Security=SSPI;";

        static long customer_id;
        static IDbRepositorySession repository_session;
        static IRepository<Customer> customers;
        static Customer customer;
        static string original_federtaion_statemnet;

        Cleanup after_each = () =>
        {
            FederationCommand.FederationStatement = original_federtaion_statemnet;
            repository_session.Dispose();
            MsSqlHelper.DropDatabase(ConnectionString);
        };

        Establish context = () =>
        {
            MsSqlHelper.BuildDatabase(ConnectionString, @"_Repositories\_FederatedPartitionProvider\create-federation.sql");

            customer_id = 29660;

            // the test simulates the federation switch, it's possible to test against real federation: you will need 
            //  * to uncomment corresponding statements in create-federation.sql
            //  * comment out next two lines
            //  * change the expected FirstName and LastName below
            //  * provide real connection string to Sql Azure instance
            original_federtaion_statemnet = FederationCommand.FederationStatement;
            FederationCommand.FederationStatement = "UPDATE [dbo].[Customers] SET [FirstName] = '{0}', [LastName]= '{1}' WHERE [CustomerId] = {2}";

            Database.SetInitializer<RepositoryAggregateRoot<Customer>>(null);

            var partitionKeyProvider = new Mock<IPartitionKeyProvider>();
            partitionKeyProvider.Setup(x => x.GetPartitionKey()).Returns(customer_id);

            var partitionProxy = new PartitionProxy(
                partitionKeyProvider.Object,
                new FederatedPartitionProvider(new DatabaseConnectionString(ConnectionString), "CustomerFederation", "cid"));

            var contextFactory = new PartitionedDbContextFactory<RepositoryAggregateRoot<Customer>>(partitionProxy);

            repository_session = new RepositorySession(contextFactory);

            customers = new Repository<Customer>(repository_session);
        };

        Because of =
            () => customer = customers.Get(customer_id);

        It should_fetch_expected_customer = () => customer.ShouldBeSimilar(new Customer
        {
            CustomerId = customer_id,
            Title = "Mr.",
            FirstName = "CustomerFederation",
            MiddleName = null,
            LastName = "cid",
            Suffix = null,
            CompanyName = "Extreme Riding Supplies",
            SalesPerson = @"adventure-works\linda3",
            EmailAddress = "anthony0@adventure-works.com",
            Phone = "429-555-0145"
        });
    }
}
