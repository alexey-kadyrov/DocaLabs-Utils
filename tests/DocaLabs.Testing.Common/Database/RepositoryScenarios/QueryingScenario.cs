using System;

namespace DocaLabs.Testing.Common.Database.RepositoryScenarios
{
    public class QueryingScenario : RepositoryTestsScenarioBase
    {
        public readonly Guid Tile1Id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public readonly Guid Tile2Id = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public readonly Guid Tile3Id = Guid.Parse("00000000-0000-0000-0000-000000000003");

        public QueryingScenario()
        {
            MsSqlDatabaseBuilder.ExecuteScripts(ConnectionString, @"populate-database-for-query-tests.sql");
        }
    }
}