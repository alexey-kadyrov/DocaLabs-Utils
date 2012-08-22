using System;
using System.Configuration;
using System.Data.SqlClient;
using DocaLabs.Storage.Core.Integration.Tests.DummyProvider;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;

namespace DocaLabs.Storage.Core.Integration.Tests.Utils
{
    [Subject(typeof(DbConnectionString)), IntegrationTag]
    class when_db_connection_string_is_newed_using_overload_constructor_with_provider_and_connection_string
    {
        static DbConnectionString connection_string;

        Because of =
            () => connection_string = new DbConnectionString("DummyProviderFactory", "connection-string");

        It should_return_specified_provider_name =
            () => connection_string.ProviderName.ShouldEqual("DummyProviderFactory");

        It should_return_specified_connection_string =
            () => connection_string.ConnectionString.ShouldEqual("connection-string");

        It should_create_db_connection_object_using_specified_provider =
            () => connection_string.CreateDbConnection().ShouldBeOfType<DummyDbConnection>();
    }

    [Subject(typeof(DbConnectionString)), IntegrationTag]
    class when_db_connection_string_is_newed_using_overload_constructor_with_connection_string_only
    {
        static DbConnectionString connection_string;

        Because of =
            () => connection_string = new DbConnectionString(@"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsKeyMapPartitionProviderTests;Integrated Security=SSPI;");

        It should_return_null_provider_name =
            () => connection_string.ProviderName.ShouldBeNull();

        It should_return_specified_connection_string =
            () => connection_string.ConnectionString.ShouldEqual(@"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsKeyMapPartitionProviderTests;Integrated Security=SSPI;");

        It should_create_sql_connection_object =
            () => connection_string.CreateDbConnection().ShouldBeOfType<SqlConnection>();
    }

    [Subject(typeof(DbConnectionString)), IntegrationTag]
    class when_db_connection_string_is_newed_using_overload_constructor_with_null_provider_and_connection_string
    {
        static DbConnectionString connection_string;

        Because of =
            () => connection_string = new DbConnectionString(null, @"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsKeyMapPartitionProviderTests;Integrated Security=SSPI;");

        It should_return_specified_provider_name =
            () => connection_string.ProviderName.ShouldBeNull();

        It should_return_specified_connection_string =
            () => connection_string.ConnectionString.ShouldEqual(@"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsKeyMapPartitionProviderTests;Integrated Security=SSPI;");

        It should_create_sql_connection_object =
            () => connection_string.CreateDbConnection().ShouldBeOfType<SqlConnection>();
    }

    [Subject(typeof(DbConnectionString)), IntegrationTag]
    class when_db_connection_string_is_newed_using_overload_constructor_with_connection_string_settings
    {
        static ConnectionStringSettings connection_settings;
        static DbConnectionString connection_string;

        Establish context =
            () =>
                connection_settings = new ConnectionStringSettings("name-1", "connection-string", "DummyProviderFactory");
        Because of =
            () => connection_string = new DbConnectionString(connection_settings);

        It should_return_specified_provider_name =
            () => connection_string.ProviderName.ShouldEqual(connection_settings.ProviderName);

        It should_return_specified_connection_string =
            () => connection_string.ConnectionString.ShouldEqual(connection_settings.ConnectionString);

        It should_create_db_connection_object_using_specified_provider =
            () => connection_string.CreateDbConnection().ShouldBeOfType<DummyDbConnection>();
    }

    [Subject(typeof(DbConnectionString)), IntegrationTag]
    class when_db_connection_string_is_newed_using_overload_constructor_with_null_provider_and_null_connection_string
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new DbConnectionString(null, null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_connection_string_argument =
            () => ((ArgumentNullException) actual_exception).ParamName.ShouldEqual("connectionString");
    }

    [Subject(typeof(DbConnectionString)), IntegrationTag]
    class when_db_connection_string_is_newed_using_overload_constructor_with_null_connection_string
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new DbConnectionString((string)null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_connection_string_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("connectionString");
    }

    [Subject(typeof(DbConnectionString)), IntegrationTag]
    class when_db_connection_string_is_newed_using_overload_constructor_with_null_connection_string_settings
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new DbConnectionString((ConnectionStringSettings)null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_connection_settings_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("connectionSettings");
    }

    [Subject(typeof(DbConnectionString)), IntegrationTag]
    class when_db_connection_string_is_newed_using_overload_constructor_with_connection_string_settings_which_has_blank_connection_string
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new DbConnectionString(new ConnectionStringSettings("name-1", "")));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_connection_settings_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("connectionSettings");
    }
}
