using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Transactions;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.Database;
using DocaLabs.Testing.Common.Database.RepositoryScenarios;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using Machine.Specifications.Annotations;
using Moq;
using Moq.Protected;
using NHibernate;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.NHibernate.Intergration.Tests
{
    class RepositoryTestsContext : RepositoryTestsContextBase
    {
        static protected ISessionManager session_manager;
        static protected IRepository<Tile> tiles;

        [UsedImplicitly] Establish before_each = () =>
        {
            SetupSessionFactory();

            session_manager = new DefaultSessionManager(
                session_factory, new DefaultDbConnectionWrapper(new DbConnectionString(RepositoryTestsScenarioBase.ConnectionString)));

            tiles = new Repository<Tile>(session_manager);
        };

        [UsedImplicitly] Cleanup after_each = () =>
        {
            if (session_manager != null)
                session_manager.Dispose();
        };
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_adding_entities_in_transaction_scope : RepositoryTestsContext
    {
        static AddingEntitiesScenario scenario;

        Establish context =
            () => scenario = new AddingEntitiesScenario();

        Because of = () =>
        {
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

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_saving_changes_before_completing_transaction_scope : RepositoryTestsContext
    {
        static AddingEntitiesScenario scenario;
        static bool is_empty_before_commit;

        Establish context =
            () => scenario = new AddingEntitiesScenario();

        Because of = () =>
        {
            using (var scope = new TransactionScope())
            {
                var tile = scenario.GenerateTile();

                tiles.Add(tile);

                tiles.Unit.SaveChanges();

                is_empty_before_commit = scenario.IsDatabaseEmpty();

                scope.Complete();
            }
        };

        It should_add_entities =
            () => scenario.ShouldAddEntities();

        It changes_should_not_appear_in_database_before_completing_transaction_scope =
            () => is_empty_before_commit.ShouldBeTrue();
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_transaction_scope_is_not_completed : RepositoryTestsContext
    {
        static AddingEntitiesScenario scenario;

        Establish context =
            () => scenario = new AddingEntitiesScenario();

        Because of = () =>
        {
            using (new TransactionScope())
            {
                var tile = scenario.GenerateTile();

                tiles.Add(tile);
            }
        };

        It data_in_database_should_not_be_changed =
            () => scenario.DatabaseShouldBeEmpty();
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_removing_entities : RepositoryTestsContext
    {
        static RemovingEntitiesScenario scenario;

        Establish context =
            () => scenario = new RemovingEntitiesScenario();

        Because of = () =>
        {
            using (var scope = new TransactionScope())
            {
                var tile = tiles.Find(scenario.KnownTileId);

                tiles.Remove(tile);

                scope.Complete();
            }
        };

        It should_remove_entities =
            () => scenario.DatabaseShouldBeEmpty();
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_updating_entities : RepositoryTestsContext
    {
        static UpdateEntitiesScenario scenario;

        Establish context =
            () => scenario = new UpdateEntitiesScenario();

        Because of = () =>
        {
            using (var scope = new TransactionScope())
            {
                var tile = tiles.Find(scenario.KnownTileId);

                scenario.GetEntities(tile);

                scenario.UpdateEntities();

                scope.Complete();
            }
        };

        It should_update_entities =
            () => scenario.ShouldUpdateEntities();
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class updating_entities_when_they_are_changed_after_reading : RepositoryTestsContext
    {
        static UpdateEntitiesScenario scenario;
        static Exception actual_exception;

        Establish context =
            () => scenario = new UpdateEntitiesScenario();

        Because of = () =>
        {
            var tile = tiles.Find(scenario.KnownTileId);

            scenario.GetEntities(tile);

            scenario.UpdateEntities();

            MsSqlDatabaseBuilder.ExecuteNonQuery(RepositoryTestsScenarioBase.ConnectionString, "update [dbo].[InterestingPoints] set [Category] = 'Dark blue' where [Category] = 'Blue'");
            MsSqlDatabaseBuilder.ExecuteNonQuery(RepositoryTestsScenarioBase.ConnectionString, "update [dbo].[Places] set [Name] = 'Place 22' where [Name] = 'Place 2'");

            try
            {
                tiles.Unit.SaveChanges();
            }
            catch (StaleObjectStateException e)
            {
                actual_exception = e;

                scenario.Refresh((IRefreshableRepository<Tile>)tiles);

                scenario.UpdateEntities();

                tiles.Unit.SaveChanges();
            }
            catch (Exception e)
            {
                actual_exception = e;
            }
        };

        It should_update_entities_after_refreshing_them =
            () => scenario.ShouldUpdateEntities();

        It should_notify_about_optimistic_concurrency_conflict_using_stale_object_state_exception =
            () => actual_exception.ShouldBeOfType<StaleObjectStateException>();
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_finding_entity_by_its_primary_key : RepositoryTestsContext
    {
        static FindEntitiesByPrimaryKeyScenario scenario;
        static Tile tile;

        Establish context =
            () => scenario = new FindEntitiesByPrimaryKeyScenario();

        Because of = () =>
        {
            tile = tiles.Find(scenario.KnownTileId);
        };

        It should_fetch_entity =
            () => scenario.ShouldLoadEntities(tile);
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_finding_entity_by_null_primary_key : RepositoryTestsContext
    {
        static FindEntitiesByPrimaryKeyScenario scenario;
        static Exception actual_exception;

        Establish context =
            () => scenario = new FindEntitiesByPrimaryKeyScenario();

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => tiles.Find(null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_key_values_argument =
            () => ((ArgumentNullException) actual_exception).ParamName.ShouldEqual("keyValues");
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_querying_for_collection_of_entities : RepositoryTestsContext
    {
        static QueryingScenario scenario;
        static List<Tile> result;

        Establish context =
            () => scenario = new QueryingScenario();

        Because of = () =>
        {
            result = tiles.Where(x => x.Id == scenario.Tile1Id || x.Id == scenario.Tile3Id).ToList();
        };

        It should_fetch_entities =
            () => result.ShouldContainOnlySimilar((x, y) => x.Id == y, scenario.Tile1Id, scenario.Tile3Id);

        // the explicit check on the property is needed as NHibertnate implementation manages to avoid using that property.
        It queryable_should_use_tile_type =
            () => tiles.ElementType.ShouldEqual(typeof (Tile));
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_querying_for_single_entity : RepositoryTestsContext
    {
        static QueryingScenario scenario;
        static Tile result;

        Establish context =
            () => scenario = new QueryingScenario();

        Because of = () =>
        {
            result = tiles.SingleOrDefault(x => x.Id == scenario.Tile2Id);
        };

        It should_fetch_entity =
            () => result.Id.ShouldEqual(scenario.Tile2Id);
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_calling_query_method : RepositoryTestsContext
    {
        static QueryingScenario scenario;
        static int result;

        Establish context =
            () => scenario = new QueryingScenario();

        Because of = () =>
        {
            result = tiles.Count();
        };

        It should_call_the_method =
            () => result.ShouldEqual(3);
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_enumerating_through_query_result_using_ienumerable_interface : RepositoryTestsContext
    {
        static QueryingScenario scenario;
        static IEnumerator<Tile> result;

        Establish context =
            () => scenario = new QueryingScenario();

        Because of = () =>
        {
            result = tiles.GetEnumerator();
        };

        It should_fetch_entities =
            () => result.ShouldContainOnlySimilar((x, y) => x.Id == y,scenario.Tile1Id, scenario.Tile2Id, scenario.Tile3Id);
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_enumerating_through_query_result_using_explicit_ienumerable_interface : RepositoryTestsContext
    {
        static QueryingScenario scenario;
        static IEnumerator result;

        Establish context =
            () => scenario = new QueryingScenario();

        Because of = () =>
        {
            result = ((IEnumerable)tiles).GetEnumerator();
        };

        It should_fetch_entities =
            () => result.ShouldContainOnlySimilar((x, y) => ((Tile)x).Id.Equals(y), scenario.Tile1Id, scenario.Tile2Id, scenario.Tile3Id);
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_repository_is_newed_with_null_session_manager
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new Repository<Tile>(null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_session_manager_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("sessionManager");
    }

    [Subject(typeof(SessionContext)), IntegrationTag]
    class when_session_context_is_newed_with_null_session
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new SessionContext(null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_session_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("session");
    }

    [Subject(typeof(DefaultSessionManager)), IntegrationTag]
    class when_disposing_session_manager_several_times : RepositoryTestsContext
    {
        static ISessionContext session_context;
        static bool result;

        Establish context = () =>
        {
            session_context = session_manager.OpenSession();
            result = false;
        };

        Because of = () =>
        {
            session_manager.Dispose();
            session_manager.Dispose();
            result = true;
        };

        It should_not_throw_any_exceptions =
            () => result.ShouldBeTrue();
    }

    [Subject(typeof(SessionContext)), IntegrationTag]
    class when_disposing_session_context_several_times : RepositoryTestsContext
    {
        static ISessionContext session_context;
        static bool result;

        Establish context = () =>
        {
            session_context = session_manager.OpenSession();
            result = false;
        };

        Because of = () =>
        {
            session_context.Dispose();
            session_context.Dispose();
            session_manager.Dispose();
            result = true;
        };

        It should_not_throw_any_exceptions =
            () => result.ShouldBeTrue();
    }

    [Subject(typeof(DefaultSessionManager)), IntegrationTag]
    class when_session_manager_is_newed_with_null_session_factory
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new DefaultSessionManager(
                null, new DefaultDbConnectionWrapper(new DbConnectionString(RepositoryTestsScenarioBase.ConnectionString))));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_session_factory_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("sessionFactory");
    }

    [Subject(typeof(DefaultSessionManager)), IntegrationTag]
    class when_session_manager_is_newed_with_null_db_connection_string
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new DefaultSessionManager(new Mock<ISessionFactory>().Object, null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_connection_string_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("connectionWrapper");
    }

    [Subject(typeof(PartitionedSessionManager)), IntegrationTag]
    class when_partitioned_session_manager_is_used
    {
        static Mock<DbConnection> mock_connection;
        static Mock<IDbConnectionWrapper> mock_connection_wrapper; 
        static Mock<ISession> mock_session;
        static Mock<ISessionFactory> mock_session_factory;
        static Mock<IPartitionProxy> mock_partition_proxy;
        static PartitionedSessionManager session_manager;

        Cleanup after =
            () => CurrentPartitionProxy.Current = null;

        Establish context = () =>
        {
            mock_connection = new Mock<DbConnection>();

            mock_connection_wrapper = new Mock<IDbConnectionWrapper>();
            mock_connection_wrapper.Setup(x => x.Connection).Returns(mock_connection.Object);

            mock_session = new Mock<ISession>();

            mock_session_factory = new Mock<ISessionFactory>();
            mock_session_factory.Setup(x => x.OpenSession(mock_connection.Object)).Returns(mock_session.Object);

            mock_partition_proxy = new Mock<IPartitionProxy>();
            mock_partition_proxy.Setup(x => x.GetConnection()).Returns(mock_connection_wrapper.Object);

            CurrentPartitionProxy.Current = mock_partition_proxy.Object;

            session_manager = new PartitionedSessionManager(mock_session_factory.Object);
        };

        Because of =
            () => session_manager.OpenSession();

        It should_get_connection_from_the_current_parrtiton_proxy =
            () => mock_partition_proxy.Verify(x => x.GetConnection(), Times.AtLeastOnce());
    }

    [Subject(typeof(PartitionedSessionManager)), IntegrationTag]
    class when_partitioned_session_manager_is_newed_with_null_session_factory
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new PartitionedSessionManager(null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_session_factory_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("sessionFactory");
    }

    [Subject(typeof(SessionManagerBase)), IntegrationTag]
    class when_open_session_fails_the_opened_connection_should_be_disposed
    {
        class TestException : Exception
        {
        }

        static Mock<DbConnection> mock_connection;
        static Mock<IDbConnectionWrapper> mock_connection_wrapper; 
        static Mock<ISessionFactory> mock_session_factory;
        static Mock<SessionManagerBase> mock_session_manager;

        static Exception actual_exception;

        Establish context = () =>
        {
            mock_connection = new Mock<DbConnection>();

            mock_connection_wrapper = new Mock<IDbConnectionWrapper>();
            mock_connection_wrapper.Setup(x => x.Connection).Returns(mock_connection.Object);

            mock_session_factory = new Mock<ISessionFactory>();
            mock_session_factory.Setup(x => x.OpenSession(mock_connection.Object)).Throws(new TestException());

            mock_session_manager = new Mock<SessionManagerBase>(mock_session_factory.Object)
            {
                CallBase = true
            };

            mock_session_manager.Protected()
                .Setup<IDbConnectionWrapper>("GetConnection")
                .Returns(mock_connection_wrapper.Object);
        };

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => mock_session_manager.Object.OpenSession());
        };

        It should_throw_test_exception =
            () => actual_exception.ShouldBeOfType<TestException>();

        It should_open_connection =
            () => mock_connection.Verify(x => x.Open(), Times.AtLeastOnce());

        // Verify on Component 'protected virtual void Dispose(bool disposing)'
        It should_dispose_opened_connection =
            () => mock_connection_wrapper.Verify(x => x.Dispose(), Times.AtLeastOnce());
    }

    [Subject(typeof(SessionManagerBase)), IntegrationTag]
    class when_session_manager_base_is_disposed_it_disposes_all_allocated_resources
    {
        static Mock<DbConnection> mock_connection;
        static Mock<IDbConnectionWrapper> mock_connection_wrapper;
        static Mock<ISession> mock_session;
        static Mock<ISessionFactory> mock_session_factory;
        static Mock<SessionManagerBase> mock_session_manager;

        Establish context = () =>
        {
            mock_connection = new Mock<DbConnection>();

            mock_connection_wrapper = new Mock<IDbConnectionWrapper>();
            mock_connection_wrapper.Setup(x => x.Connection).Returns(mock_connection.Object);

            mock_session = new Mock<ISession>();

            mock_session_factory = new Mock<ISessionFactory>();
            mock_session_factory.Setup(x => x.OpenSession(mock_connection.Object)).Returns(mock_session.Object);

            mock_session_manager = new Mock<SessionManagerBase>(mock_session_factory.Object)
            {
                CallBase = true
            };

            mock_session_manager.Protected()
                .Setup<IDbConnectionWrapper>("GetConnection")
                .Returns(mock_connection_wrapper.Object);

            mock_session_manager.Object.OpenSession();
        };

        Because of =
            () => mock_session_manager.Object.Dispose();

        // Verify on Component 'protected virtual void Dispose(bool disposing)'
        It should_dispose_opened_connection =
            () => mock_connection_wrapper.Verify(x => x.Dispose(), Times.AtLeastOnce());

        It should_dispose_opened_session =
            () => mock_session.Verify(x => x.Dispose(), Times.AtLeastOnce());
    }
}
