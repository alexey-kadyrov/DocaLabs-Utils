using System;
using DocaLabs.Storage.Core.Repositories.DataService;
using DocaLabs.Storage.Integration.Tests._Repositories._DataService._Utils;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Integration.Tests._Repositories._DataService
{
    [Subject(typeof(DataServiceQuery<>))]
    class when_data_service_scalar_query_is_executed_with_null_repository
    {
        static Mock<DataServiceScalarQuery<Product, int>> query;
        static Exception actual_exception;

        Establish context =
            () => query = new Mock<DataServiceScalarQuery<Product, int>> { CallBase = true };

        Because of =
            () => actual_exception = Catch.Exception(() => query.Object.Execute(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_repository_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("repository");
    }
}
