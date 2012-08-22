using System;
using System.Data.Common;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.Database;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;

namespace DocaLabs.Storage.Core.Integration.Tests.Utils
{
    [Subject(typeof(StoredProcedureExtensions)), IntegrationTag]
    class when_open_command_is_called_with_null_db_connection_argument
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => ((DbConnection) null).OpenCommand("my-sproc"));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_connection_argument =
            () => ((ArgumentNullException) actual_exception).ParamName.ShouldEqual("connection");
    }

    [Subject(typeof(StoredProcedureExtensions)), IntegrationTag]
    class when_open_command_is_called_with_null_db_stored_procedure_argument
    {
        static Exception actual_exception;
        static DbConnection db_connection;

        Establish context =
            () => db_connection = new DbConnectionString(@"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsKeyMapPartitionProviderTests;Integrated Security=SSPI;").CreateDbConnection();

        Because of =
            () => actual_exception = Catch.Exception(() => db_connection.OpenCommand(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_stored_procedure_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("storedProcedure");
    }

    [Subject(typeof(StoredProcedureExtensions)), IntegrationTag]
    class when_add_input_parameter_is_called_with_null_command_argument
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => ((DbCommand)null).AddInputParameter("someValue", 42));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_command_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("command");
    }

    [Subject(typeof(StoredProcedureExtensions)), IntegrationTag]
    class when_with_is_called_with_null_command_argument
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => ((DbCommand)null).With("someValue", 42));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_command_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("command");
    }

    [Subject(typeof(StoredProcedureExtensions)), IntegrationTag]
    class when_add_return_parameter_is_called_with_null_command_argument
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => ((DbCommand)null).AddReturnParameter());

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_command_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("command");
    }

    [Subject(typeof(StoredProcedureExtensions)), IntegrationTag]
    class when_get_return_value_is_called_with_null_command_argument
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => ((DbCommand)null).GetReturnValue());

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_command_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("command");
    }

    [Subject(typeof(StoredProcedureExtensions)), IntegrationTag]
    class when_return_is_called_with_null_command_argument
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => ((DbCommand)null).Return());

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_command_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("command");
    }

    [Subject(typeof(StoredProcedureExtensions)), IntegrationTag]
    class when_execute_non_query_is_called_with_null_command_argument
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => ((DbCommand)null).Execute());

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_command_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("command");
    }

    [Subject(typeof (StoredProcedureExtensions)), IntegrationTag]
    class when_calling_to_stored_procedure
    {
        public const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsKeyMapPartitionProviderTests;Integrated Security=SSPI;";

        Cleanup after =
            () => MsSqlDatabaseBuilder.Drop(ConnectionString);

        Establish context = 
            () => MsSqlDatabaseBuilder.Build(ConnectionString, "Partitioning/stored-procedure-extensions-tests-support.sql");

        It should_be_able_to_call_the_stored_procedure_with_one_of_parameters_as_null_using_chained_calls = () =>
        {
            using (var connection = new DbConnectionString(ConnectionString).CreateDbConnection())
            using (var command = connection.OpenCommand("RunTest")
                .With("param1", 42)
                .With<string>("param2", null)
                .Return())
            {
                command.Execute().ShouldEqual(200);
            }
        };

        It should_be_able_to_call_the_stored_procedure_using_chained_calls = () =>
        {
            using (var connection = new DbConnectionString(ConnectionString).CreateDbConnection())
            using (var command = connection.OpenCommand("RunTest")
                .With("param1", 42)
                .With("param2", "something")
                .Return())
            {
                command.Execute().ShouldEqual(42);
            }
        };

        It should_be_able_to_call_the_stored_procedure_with_one_of_parameters_as_null_using_direct_calls = () =>
        {
            using (var connection = new DbConnectionString(ConnectionString).CreateDbConnection())
            using (var command = connection.OpenCommand("RunTest"))
            {
                command.AddInputParameter("param1", 42);
                command.AddInputParameter<string>("param2", null);
                command.AddReturnParameter();

                command.Execute().ShouldEqual(200);
            }
        };

        It should_be_able_to_call_the_stored_procedure_using_direct_calls = () =>
        {
            using (var connection = new DbConnectionString(ConnectionString).CreateDbConnection())
            using (var command = connection.OpenCommand("RunTest"))
            {
                command.AddInputParameter("param1", 42);
                command.AddInputParameter("param2", "something");
                command.AddReturnParameter();

                command.Execute().ShouldEqual(42);
            }
        };
    }
}
