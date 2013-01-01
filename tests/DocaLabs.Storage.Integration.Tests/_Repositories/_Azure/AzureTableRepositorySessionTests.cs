using System;
using DocaLabs.AzureStorage.Tables;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Azure
{
    [Subject(typeof(AzureTableRepositorySession))]
    class when_azure_table_repository_session_is_newed_with_null_table
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new AzureTableRepositorySession(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_table_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("table");
    }
}
