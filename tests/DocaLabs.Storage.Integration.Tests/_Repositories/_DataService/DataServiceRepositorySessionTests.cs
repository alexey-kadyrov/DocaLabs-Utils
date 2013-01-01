using System;
using DocaLabs.Storage.Core.Repositories.DataService;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Integration.Tests._Repositories._DataService
{
    [Subject(typeof(DataServiceRepositorySession))]
    class when_data_service_repository_session_is_newed_with_null_service_root
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new DataServiceRepositorySession(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_service_root_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("serviceRoot");
    }
}
