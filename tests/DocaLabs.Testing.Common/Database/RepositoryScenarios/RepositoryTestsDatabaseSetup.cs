using System;

namespace DocaLabs.Testing.Common.Database.RepositoryScenarios
{
    public class RepositoryTestsDatabaseSetup
    {
        static bool IsCreated { get; set; }

        public static bool EnsureDatabaseExist()
        {
            if(IsCreated)
                return false;

            AppDomain.CurrentDomain.DomainUnload += HandleDomainUnload;

            MsSqlDatabaseBuilder.Build(RepositoryTestsScenarioBase.ConnectionString, @"create-tables.sql");

            return true;
        }

        static void HandleDomainUnload(object sender, EventArgs e)
        {
            IsCreated = false;

            MsSqlDatabaseBuilder.Drop(RepositoryTestsScenarioBase.ConnectionString);

            AppDomain.CurrentDomain.DomainUnload -= HandleDomainUnload;
        }
    }
}
