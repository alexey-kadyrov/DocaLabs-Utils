namespace DocaLabs.Testing.Common.Database.RepositoryScenarios
{
    public class RemovingEntitiesScenario : RepositoryTestsScenarioBase
    {
        public RemovingEntitiesScenario()
        {
            MsSqlDatabaseBuilder.ExecuteScripts(ConnectionString, @"populate-database.sql");
        }
    }
}
