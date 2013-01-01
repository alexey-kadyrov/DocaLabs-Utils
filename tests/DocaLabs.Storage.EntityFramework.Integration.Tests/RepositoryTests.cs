using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Repositories;
using DocaLabs.Testing.Common.Database;
using DocaLabs.Testing.Common.Database.RepositoryScenarios;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using Machine.Specifications.Annotations;
using Moq;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.EntityFramework.Integration.Tests
{
    class RepositoryTestsContext
    {
        static protected IDbConnectionManager connection_manager;
        static protected IRepository<Tile> tiles;

        [UsedImplicitly] Cleanup after_each = () =>
        {
            if (connection_manager != null)
                connection_manager.Dispose();

            RepositoryTestsScenarioBase.CleanAfter();
        };

        [UsedImplicitly] Establish before_each = () =>
        {
            if (RepositoryTestsDatabaseSetup.EnsureDatabaseExist())
            {
                MsSqlDatabaseBuilder.ExecuteScripts(RepositoryTestsScenarioBase.ConnectionString, @"add-cascade-delete.sql");
            }

            RepositoryConfiguration.RemoveInitializer<Tile>();

            connection_manager = new DefaultDbConnectionManager(new DatabaseConnection(
                new DatabaseConnectionString(RepositoryTestsScenarioBase.ConnectionString)));

            tiles = new Repository<Tile>(connection_manager);
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

                tiles.Context.SaveChanges();

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

                tiles.Context.SaveChanges();

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

                tiles.Context.SaveChanges();
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
                var tile = tiles.Get(scenario.KnownTileId);

                tiles.Remove(tile);

                tiles.Context.SaveChanges();

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
                var tile = tiles.Get(scenario.KnownTileId);

                scenario.GetEntities(tile);

                scenario.UpdateEntities();

                tiles.Context.SaveChanges();

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
            var tile = tiles.Get(scenario.KnownTileId);

            scenario.GetEntities(tile);

            scenario.UpdateEntities();

            MsSqlDatabaseBuilder.ExecuteNonQuery(RepositoryTestsScenarioBase.ConnectionString, "update [dbo].[InterestingPoints] set [Category] = 'Dark blue' where [Category] = 'Blue'");
            MsSqlDatabaseBuilder.ExecuteNonQuery(RepositoryTestsScenarioBase.ConnectionString, "update [dbo].[Places] set [Name] = 'Place 22' where [Name] = 'Place 2'");

            try
            {
                tiles.Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                actual_exception = e;

                scenario.Refresh((IRefreshableRepository<Tile>)tiles);

                scenario.UpdateEntities();

                tiles.Context.SaveChanges();
            }
            catch (Exception e)
            {
                actual_exception = e;
            }
        };

        It should_update_entities_after_refreshing_them =
            () => scenario.ShouldUpdateEntities();

        It should_notify_about_optimistic_concurrency_conflict_using_db_update_concurrency_exception =
            () => actual_exception.ShouldBeOfType<DbUpdateConcurrencyException>();
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
            tile = tiles.Get(scenario.KnownTileId);
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
            actual_exception = Catch.Exception(() => tiles.Get(null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_key_values_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("keyValues");
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_finding_entity_by_compound_primary_key : RepositoryTestsContext
    {
        static FindEntitiesByPrimaryKeyScenario scenario;
        static Exception actual_exception;

        Establish context =
            () => scenario = new FindEntitiesByPrimaryKeyScenario();

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => tiles.Get(Guid.NewGuid(), Guid.NewGuid()));
        };

        It should_throw_argument_exception =
            () => actual_exception.ShouldBeOfType<ArgumentException>();

        It should_report_key_values_argument =
            () => ((ArgumentException)actual_exception).ParamName.ShouldEqual("keyValues");
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
            () => tiles.ElementType.ShouldEqual(typeof(Tile));
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
            () => result.ShouldContainOnlySimilar((x, y) => x.Id == y, scenario.Tile1Id, scenario.Tile2Id, scenario.Tile3Id);
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
    class when_repository_with_configured_on_model_creating_action_is_used
    {
        static IDbConnectionManager connection_manager;
        static IRepository<InterestingPoint> interesting_points;
        static bool is_action_called;

        Cleanup after = () =>
        {
            if (connection_manager != null)
                connection_manager.Dispose();

            RepositoryTestsScenarioBase.CleanAfter();
        };

        Establish context = () =>
        {
            is_action_called = false;

            RepositoryTestsDatabaseSetup.EnsureDatabaseExist();

            RepositoryConfiguration.RemoveInitializer<InterestingPoint>();
            RepositoryConfiguration.SetOnModelCreatingAction<InterestingPoint>(m => is_action_called = true);

            connection_manager = new DefaultDbConnectionManager(new DatabaseConnection(
                new DatabaseConnectionString(RepositoryTestsScenarioBase.ConnectionString)));

            interesting_points = new Repository<InterestingPoint>(connection_manager);
        };

        Because of = () => 
            interesting_points.Add(new InterestingPoint { Id = Guid.NewGuid(), Category = "White" });

        It should_call_the_supplied_action =
            () => is_action_called.ShouldBeTrue();
    }

    [Subject(typeof(Repository<>)), IntegrationTag]
    class when_repository_is_newed_with_null_session_context_manager
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new Repository<Tile>(null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_session_context_manager_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("dbConnectionManager");
    }

    [Subject(typeof(DefaultDbConnectionManager)), IntegrationTag]
    class when_session_context_manager_is_newed_with_null_connection_string
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new DefaultDbConnectionManager(null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_connection_string_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("connection");
    }

    [Subject(typeof(PartitionedDbConnectionManager)), IntegrationTag]
    class when_partitioned_session_context_manager_is_used
    {
        static Mock<IDatabaseConnection> mock_connection_wrapper; 
        static Mock<DbConnection> mock_connection;
        static Mock<IPartitionProxy> mock_partition_proxy;
        static PartitionedDbConnectionManager connection_manager;

        Cleanup after =
            () => CurrentPartitionProxy.Current = null;

        Establish context = () =>
        {
            mock_connection = new Mock<DbConnection>();

            mock_connection_wrapper = new Mock<IDatabaseConnection>();
            mock_connection_wrapper.Setup(x => x.Connection).Returns(mock_connection.Object);

            mock_partition_proxy = new Mock<IPartitionProxy>();
            mock_partition_proxy.Setup(x => x.GetConnection()).Returns(mock_connection_wrapper.Object);

            CurrentPartitionProxy.Current = mock_partition_proxy.Object;

            connection_manager = new PartitionedDbConnectionManager();
        };

        Because of =
            () => connection_manager.OpenConnection();

        It should_get_connection_from_the_current_parrtiton_proxy =
            () => mock_partition_proxy.Verify(x => x.GetConnection(), Times.AtLeastOnce());
    }

    [Subject(typeof(DbConnectionManagerBase)), IntegrationTag]
    class when_session_manager_base_is_disposed_it_disposes_all_allocated_resources
    {
        static Mock<DbConnection> mock_connection; 
        static Mock<IDatabaseConnection> mock_connection_wrapper;
        static Mock<DbConnectionManagerBase> mock_connection_manager;

        Establish context = () =>
        {
            mock_connection = new Mock<DbConnection>();

            mock_connection_wrapper = new Mock<IDatabaseConnection>();
            mock_connection_wrapper.Setup(x => x.Connection).Returns(mock_connection.Object);

            mock_connection_manager = new Mock<DbConnectionManagerBase>(mock_connection_wrapper.Object)
            {
                CallBase = true
            };

            mock_connection_manager.Object.OpenConnection();
        };

        Because of =
            () => mock_connection_manager.Object.Dispose();

        // Verify on Component 'protected virtual void Dispose(bool disposing)'
        It should_dispose_opened_connection =
            () => mock_connection_wrapper.Verify(x => x.Dispose(), Times.AtLeastOnce());
    }

    [Subject(typeof(DefaultDbConnectionManager)), IntegrationTag]
    class when_disposing_session_manager_several_times : RepositoryTestsContext
    {
        static IDatabaseConnection connection;
        static bool result;

        Establish context = () =>
        {
            connection = connection_manager.OpenConnection();
            result = false;
        };

        Because of = () =>
        {
            connection_manager.Dispose();
            connection_manager.Dispose();
            result = true;
        };

        It should_not_throw_any_exceptions =
            () => result.ShouldBeTrue();
    }

    [Subject(typeof(RepositoryContext<>)), IntegrationTag]
    class when_disposing_session_context_several_times : RepositoryTestsContext
    {
        static IRepositoryContext<Tile> repository_context;
        static bool result;

        Establish context = () =>
        {
            repository_context = connection_manager.CreateRepositoryContext<Tile>();
            result = false;
        };

        Because of = () =>
        {
            repository_context.Dispose();
            repository_context.Dispose();
            connection_manager.Dispose();
            result = true;
        };

        It should_not_throw_any_exceptions =
            () => result.ShouldBeTrue();
    }
}
