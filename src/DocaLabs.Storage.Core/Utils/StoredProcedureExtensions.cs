using System;
using System.Data;
using System.Data.Common;

namespace DocaLabs.Storage.Core.Utils
{
    /// <summary>
    /// Defines useful helper extensions for ADO.NET classes.
    /// </summary>
    public static class StoredProcedureExtensions
    {
        /// <summary>
        /// Name which is used for the return parameter.
        /// </summary>
        public const string ReturnValueParameterName = "ret_val";

        /// <summary>
        /// Creates and opens a stored procedure command on the database connection.
        /// </summary>
        /// <param name="connection">DbConnection  instance.</param>
        /// <param name="storedProcedure">Stored procedure name.</param>
        /// <returns>A new instance of the DbCommand class.</returns>
        public static DbCommand OpenCommand(this DbConnection  connection, string storedProcedure)
        {
            if(connection == null)
                throw new ArgumentNullException("connection");

            if(string.IsNullOrWhiteSpace(storedProcedure))
                throw new ArgumentNullException("storedProcedure");

            var command = connection.CreateCommand();

            command.CommandText = storedProcedure;
            command.CommandType = CommandType.StoredProcedure;

            command.Connection.Open();

            return command;
        }

        /// <summary>
        /// Creates and adds a return parameter with 'ret_val' name to the command with the Int32 type.
        /// The rest of the parameter properties are default. The method is suitable for method call chaining.
        /// </summary>
        /// <param name="command">DbCommand instance.</param>
        /// <returns>The passed command.</returns>
        public static DbCommand Return(this DbCommand command)
        {
            command.AddReturnParameter();

            return command;
        }

        /// <summary>
        /// Creates and adds an input parameter to the command with specified name and value.
        /// The rest of the parameter properties are default. The method is suitable for method call chaining.
        /// </summary>
        /// <typeparam name="T">Type of the parameter to add.</typeparam>
        /// <param name="command">DbCommand instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Parameter's value.</param>
        /// <returns>The passed command.</returns>
        public static DbCommand With<T>(this DbCommand command, string parameterName, T value)
        {
            command.AddInputParameter(parameterName, value);
            return command;
        }

        /// <summary>
        /// Calls ExecuteNonQuery on the command and gets the return value.
        /// </summary>
        /// <param name="command">DbCommand instance.</param>
        public static int Execute(this DbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            command.ExecuteNonQuery();

            return command.GetReturnValue();
        }

        /// <summary>
        /// Creates and adds an input parameter to the command with specified name and value.
        /// The rest of the parameter properties are default.
        /// </summary>
        /// <typeparam name="T">Type of the parameter to add.</typeparam>
        /// <param name="command">DbCommand instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Parameter's value.</param>
        /// <returns>A new instance of the DbParameter class.</returns>
        public static DbParameter AddInputParameter<T>(this DbCommand command, string parameterName, T value)
        {
            if(command == null)
                throw new ArgumentNullException("command");

            var p = command.CreateParameter();

            p.ParameterName = parameterName;
            p.Direction = ParameterDirection.Input;
            
            if (ReferenceEquals(null, value) || ReferenceEquals(DBNull.Value, value))
            {
                p.Value = DBNull.Value;
            }
            else
            {
                p.Value = value;
            }

            command.Parameters.Add(p);

            return p;
        }

        /// <summary>
        /// Creates and adds a return parameter with 'ret_val' name to the command with the Int32 type.
        /// The rest of the parameter properties are default.
        /// </summary>
        /// <param name="command">DbCommand instance.</param>
        /// <returns>A new instance of the DbParameter class.</returns>
        public static DbParameter AddReturnParameter(this DbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            var p = command.CreateParameter();

            p.ParameterName = ReturnValueParameterName;
            p.Direction = ParameterDirection.ReturnValue;
            p.DbType = DbType.Int32;

            command.Parameters.Add(p);

            return p;
        }

        /// <summary>
        /// Gets the return value. The type of the return parameter is always Int32.
        /// </summary>
        /// <param name="command">DbCommand instance.</param>
        public static int GetReturnValue(this DbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            return (int)command.Parameters[ReturnValueParameterName].Value;
        }
    }
}
