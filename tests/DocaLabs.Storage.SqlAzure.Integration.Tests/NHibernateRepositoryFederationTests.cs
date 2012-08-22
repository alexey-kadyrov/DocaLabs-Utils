using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Storage.NHibernate;
using DocaLabs.Storage.SqlAzure.Integration.Tests.Entities;
using DocaLabs.Storage.SqlAzure.Partitioning;
using DocaLabs.Testing.Common.Database;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using Moq;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.SqlAzure.Integration.Tests
{
    [Subject("Federation tests for NHibernate Repository")]
    class when_nhibernate_repository_is_used_with_federated_database
    {
        const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsFederatedRepositoryTests;Integrated Security=SSPI;";

        static Mock<IPartitionKeyProvider> partition_key_provider;
        static ISessionFactory session_factory;
        static ISessionManager session_manager;
        static IRepository<Customer> customers;
        static Customer customer;
        static long customer_id;
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

            session_factory = SetupSessionFactory();

            session_manager = new PartitionedSessionManager(session_factory);

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

        static ISessionFactory SetupSessionFactory()
        {
            var mapper = new ModelMapper();

            mapper.AddMapping<CustomerMap>();

            var cfg = new Configuration().DataBaseIntegration(c =>
            {
                c.Dialect<MsSql2008Dialect>();
                c.ConnectionString = ConnectionString;
                c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;

                c.LogFormattedSql = true;
                c.LogSqlInConsole = true;
                c.AutoCommentSql = true;
            });

            cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

            return cfg.BuildSessionFactory();
        }

        class CustomerMap : ClassMapping<Customer>
        {
            public CustomerMap()
            {
                Table("Customers");

                DynamicUpdate(true);

                Id(x => x.CustomerId);

                Property(x => x.Title);
                Property(x => x.FirstName);
                Property(x => x.MiddleName);
                Property(x => x.LastName);
                Property(x => x.Suffix);
                Property(x => x.CompanyName);
                Property(x => x.SalesPerson);
                Property(x => x.EmailAddress);
                Property(x => x.Phone);
            }
        }

    }
}
