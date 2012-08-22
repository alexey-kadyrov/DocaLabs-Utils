using System;
using DocaLabs.Storage.Core.DataService;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;

namespace DocaLabs.Storage.Core.Integration.Tests.DataService
{
    [Subject(typeof(DataServiceStorageContextManager)), IntegrationTag]
    class when_data_service_context_manager_is_newed_with_null_service_root_argument
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new DataServiceStorageContextManager(null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_service_root_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("serviceRoot");
    }
}
