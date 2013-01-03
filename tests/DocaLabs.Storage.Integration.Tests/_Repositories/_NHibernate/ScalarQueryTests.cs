using System;
using DocaLabs.NHibernateStorage;
using DocaLabs.Storage.Integration.Tests._Repositories._Scenarios;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Integration.Tests._Repositories._NHibernate
{
    [Subject(typeof(Query<>))]
    class when_nhibernate_scalar_query_is_executed_with_null_repository
    {
        static Mock<ScalarQuery<Book, int>> query;
        static Exception actual_exception;

        Establish context =
            () => query = new Mock<ScalarQuery<Book, int>> { CallBase = true };

        Because of =
            () => actual_exception = Catch.Exception(() => query.Object.Execute(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_repository_argument =
            () => ((ArgumentNullException) actual_exception).ParamName.ShouldEqual("repository");
    }
}
