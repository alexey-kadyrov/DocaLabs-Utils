using System.Transactions;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.Database.RepositoryScenarios;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.NHibernate.Intergration.Tests
{
    [Subject(typeof(DefaultTransientRepositoryFactory)), IntegrationTag]
    class when_default_transient_repository_factory_is_used_to_add_entities : RepositoryTestsContextBase
    {
        static AddingEntitiesScenario scenario;
        static TransientRepositoryFactory factory;

        Establish context = () =>
        {
            SetupSessionFactory();

            factory = new DefaultTransientRepositoryFactory(session_factory, new DbConnectionString(RepositoryTestsScenarioBase.ConnectionString));

            scenario = new AddingEntitiesScenario();
        };

        Because of = () =>
        {
            using (var tiles = factory.Create<Tile>())
            using (var scope = new TransactionScope())
            {
                var tile = scenario.GenerateTile();

                tiles.Add(tile);

                scope.Complete();
            }
        };

        It should_add_entities =
            () => scenario.ShouldAddEntities();
    }

    [Subject(typeof(PartitionedTransientRepositoryFactory)), IntegrationTag]
    class when_partitioned_transient_repository_factory_is_used_to_add_entities : RepositoryTestsContextBase
    {
        static Mock<IPartitionProxy> mock_partition_proxy;
        static AddingEntitiesScenario scenario;
        static TransientRepositoryFactory factory;

        Cleanup after =
            () => CurrentPartitionProxy.Current = null;

        Establish context = () =>
        {
            SetupSessionFactory();

            mock_partition_proxy = new Mock<IPartitionProxy>();
            mock_partition_proxy.Setup(x => x.GetConnection()).Returns(
                new DefaultDbConnectionWrapper(new DbConnectionString(RepositoryTestsScenarioBase.ConnectionString)));

            CurrentPartitionProxy.Current = mock_partition_proxy.Object;

            factory = new PartitionedTransientRepositoryFactory(session_factory);

            scenario = new AddingEntitiesScenario();
        };

        Because of = () =>
        {
            using (var tiles = factory.Create<Tile>())
            using (var scope = new TransactionScope())
            {
                var tile = scenario.GenerateTile();

                tiles.Add(tile);

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
            ISessionManager SessionManager { get; set; }

            public TransientRepositoryFactoryBaseTestWrapper(ISessionManager sessionManager)
                : base(null)
            {
                SessionManager = sessionManager;
            }

            public override ITransientRepository<TEntity> Create<TEntity>()
            {
                return new TransientRepository<TEntity>(SessionManager);
            }
        }

        static Mock<ISessionManager> mock_session_manager;
        static TransientRepositoryFactory.ITransientRepository<Tile> tiles;

        Establish context = () =>
        {
            mock_session_manager = new Mock<ISessionManager>();

            tiles = new TransientRepositoryFactoryBaseTestWrapper(mock_session_manager.Object).Create<Tile>();
        };

        Because of =
            () => tiles.Dispose();

        It should_dispose_session_manager =
            () => mock_session_manager.Verify(x => x.Dispose(), Times.AtLeastOnce());
    }
}
