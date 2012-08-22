using System.Transactions;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.Database;
using DocaLabs.Testing.Common.Database.RepositoryScenarios;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.EntityFramework.Integration.Tests
{
    [Subject(typeof(DefaultTransientRepositoryFactory)), IntegrationTag]
    class when_default_transient_repository_factory_is_used_to_add_entities
    {
        static AddingEntitiesScenario scenario;
        static TransientRepositoryFactory factory;

        Cleanup after_each = 
            () => RepositoryTestsScenarioBase.CleanAfter();

        Establish context = () =>
        {
            if (RepositoryTestsDatabaseSetup.EnsureDatabaseExist())
                MsSqlDatabaseBuilder.ExecuteScripts(RepositoryTestsScenarioBase.ConnectionString, @"add-cascade-delete.sql");

            RepositoryConfiguration.RemoveInitializer<Tile>();

            factory = new DefaultTransientRepositoryFactory(new DbConnectionString(RepositoryTestsScenarioBase.ConnectionString));

            scenario = new AddingEntitiesScenario();
        };

        Because of = () =>
        {
            using (var tiles = factory.Create<Tile>())
            using (var scope = new TransactionScope())
            {
                var tile = scenario.GenerateTile();

                tiles.Add(tile);

                tiles.Unit.SaveChanges();
                scope.Complete();
            }
        };

        It should_add_entities =
            () => scenario.ShouldAddEntities();
    }

    [Subject(typeof(PartitionedTransientRepositoryFactory)), IntegrationTag]
    class when_partitioned_transient_repository_factory_is_used_to_add_entities
    {
        static Mock<IPartitionProxy> mock_partition_proxy;
        static AddingEntitiesScenario scenario;
        static TransientRepositoryFactory factory;

        Cleanup after = () =>
        {
            CurrentPartitionProxy.Current = null;
            RepositoryTestsScenarioBase.CleanAfter();
        };

        Establish context = () =>
        {
            if (RepositoryTestsDatabaseSetup.EnsureDatabaseExist())
                MsSqlDatabaseBuilder.ExecuteScripts(RepositoryTestsScenarioBase.ConnectionString, @"add-cascade-delete.sql");

            RepositoryConfiguration.RemoveInitializer<Tile>();

            mock_partition_proxy = new Mock<IPartitionProxy>();
            mock_partition_proxy.Setup(x => x.GetConnection()).Returns(
                new DefaultDbConnectionWrapper(new DbConnectionString(RepositoryTestsScenarioBase.ConnectionString)));

            CurrentPartitionProxy.Current = mock_partition_proxy.Object;

            factory = new PartitionedTransientRepositoryFactory();

            scenario = new AddingEntitiesScenario();
        };

        Because of = () =>
        {
            using (var tiles = factory.Create<Tile>())
            using (var scope = new TransactionScope())
            {
                var tile = scenario.GenerateTile();

                tiles.Add(tile);

                tiles.Unit.SaveChanges();
                scope.Complete();
            }
        };

        It should_add_entities =
            () => scenario.ShouldAddEntities();
    }

    [Subject("TransientRepositoryFactoryBase.TransientRepository"), IntegrationTag]
    class when_transient_repository_is_disposed
    {
        class TransientRepositoryFactoryBaseTestWrapper : TransientRepositoryFactoryBase
        {
            IDbConnectionManager ConnectionManager { get; set; }

            public TransientRepositoryFactoryBaseTestWrapper(IDbConnectionManager sessionManager)
            {
                ConnectionManager = sessionManager;
            }

            public override ITransientRepository<TEntity> Create<TEntity>()
            {
                return new TransientRepository<TEntity>(ConnectionManager);
            }
        }

        static Mock<IDbConnectionManager> mock_connection_manager;
        static TransientRepositoryFactory.ITransientRepository<Tile> tiles;

        Establish context = () =>
        {
            mock_connection_manager = new Mock<IDbConnectionManager>();

            tiles = new TransientRepositoryFactoryBaseTestWrapper(mock_connection_manager.Object).Create<Tile>();
        };

        Because of =
            () => tiles.Dispose();

        It should_dispose_session_manager =
            () => mock_connection_manager.Verify(x => x.Dispose(), Times.AtLeastOnce());
    }
}
