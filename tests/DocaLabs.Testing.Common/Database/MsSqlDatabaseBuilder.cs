using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace DocaLabs.Testing.Common.Database
{
    public static class MsSqlDatabaseBuilder
    {
        public static void Build(string connectionString, params string[] files)
        {
            CreateEmptyDatabase(connectionString);

            ExecuteScripts(connectionString, files);
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

        public static List<T> ExecuteQuery<T>(string connectionString, string query) where T : class, new()
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection) { CommandType = CommandType.Text })
            {
                command.Connection.Open();

                var list = new List<T>();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var o = new T();

                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var propertyInfo = o.GetType().GetProperty(reader.GetName(i));
                            if(propertyInfo != null)
                                propertyInfo.SetValue(o, reader[i], null);
                        }

                        list.Add(o);
                    }    
                }

                return list;
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

        public static void Drop(string connectionString)
        {
            SqlConnection.ClearAllPools();

            using (var context = new EmptyDbContext(connectionString))
            {
                var db = context.Database;

                // drop the database if exists
                db.Delete();
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
                System.Data.Entity.Database.SetInitializer<EmptyDbContext>(null);
            }

            public EmptyDbContext(string connectionString)
                : base(connectionString)
            {
            }
        }
    }
}
