using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace DocaLabs.Testing.Common
{
    public static class MsSqlHelper
    {
        public const string EfConnectionStringName = "name=DatabaseTestsConnectionString";

        public static ConnectionStringSettings ConnectionStringSettings
        {
            get { return ConfigurationManager.ConnectionStrings["DatabaseTestsConnectionString"]; }
        }

        public static void ExecuteScripts(string connectionString, params string[] files)
        {
            foreach (var query in files.SelectMany(file => FlatFileToSqlCommands(File.ReadAllText(file))))
            {
                ExecuteNonQuery(connectionString, query);
            }
        }

        public static void ExecuteNonQuery(string connectionString, string query)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection) { CommandType = CommandType.Text })
            {
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public static void BuildDatabase(string connectionString, params string[] files)
        {
            CreateEmptyDatabase(connectionString);

            ExecuteScripts(connectionString, files);
        }

        public static void DropDatabase(string connectionString)
        {
            SqlConnection.ClearAllPools();

            using (var context = new EmptyDbContext(connectionString))
            {
                var db = context.Database;

                // drop the database if exists
                db.Delete();
            }
        }

        static void CreateEmptyDatabase(string connectionString)
        {
            SqlConnection.ClearAllPools();

            using (var context = new EmptyDbContext(connectionString))
            {
                var db = context.Database;

                // drop the database if exists
                db.Delete();

                // create new database
                db.Create();
            }
        }

        static IEnumerable<string> FlatFileToSqlCommands(string flatFile)
        {
            return flatFile.Split(new[] { "\r\nGO\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        class EmptyDbContext : DbContext
        {
            static EmptyDbContext()
            {
                Database.SetInitializer<EmptyDbContext>(null);
            }

            public EmptyDbContext(string connectionString)
                : base(connectionString)
            {
            }
        }
    }
}
