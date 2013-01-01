using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace DocaLabs.Storage.Integration.Tests._Utils
{
    static class MsSqlHelper
    {
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

        static IEnumerable<string> FlatFileToSqlCommands(string flatFile)
        {
            return flatFile.Split(new[] { "\r\nGO\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
