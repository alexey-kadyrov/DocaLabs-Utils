using System;
using System.Data.Entity;
using DocaLabs.EntityFrameworkStorage;
using Machine.Specifications;
using Moq;
using Moq.Protected;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Integration.Tests._Repositories._EntityFramework
{
    [Subject(typeof(RepositorySession))]
    class wehen_newing_repository_session_with_null_session_factory
    {
        static Exception actual_exception;

        Because of = 
            () => actual_exception = Catch.Exception(() => new RepositorySession(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_context_factory_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("contextFactory");
    }

    [Subject(typeof(RepositorySession))]
    class when_disposing_repository_session_without_openning_connection
    {
        static Mock<IDbContextFactory> context_factory;
        static RepositorySession repository_session;

        Establish context = () =>
        {
            context_factory = new Mock<IDbContextFactory>();

            repository_session = new RepositorySession(context_factory.Object);
        };

        Because of =
            () => repository_session.Dispose();

        It should_not_request_new_context =
            () => context_factory.Verify(x => x.Create(), Times.Never());
    }

    [Subject(typeof(RepositorySession))]
    class when_asking_for_for_a_new_db_context_and_then_disposing_repository_session
    {
        static Mock<IDbContextFactory> context_factory;
        static Mock<DbContext> db_context;
        static DbContext returned_context;
        static RepositorySession repository_session;

        Establish context = () =>
        {
            db_context = new Mock<DbContext> { CallBase = true };

            context_factory = new Mock<IDbContextFactory>();
            context_factory.Setup(x => x.Create()).Returns(db_context.Object);

            repository_session = new RepositorySession(context_factory.Object);
        };

        Because of = () =>
        {
            returned_context = repository_session.Context;
            repository_session.Dispose();
        };

        It should_return_expected_context =
            () => returned_context.ShouldBeTheSameAs(db_context.Object);

        It should_dispose_context_session =
            () => db_context.Protected().Verify("Dispose", Times.AtLeastOnce(), true);
    }
}
