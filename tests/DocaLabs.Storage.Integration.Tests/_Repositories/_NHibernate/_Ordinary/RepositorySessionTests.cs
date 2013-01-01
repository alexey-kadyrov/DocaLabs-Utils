using System;
using DocaLabs.NHibernateStorage;
using Machine.Specifications;
using Moq;
using NHibernate;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Integration.Tests._Repositories._NHibernate._Ordinary
{
    [Subject(typeof(RepositorySession))]
    class wehen_newing_repository_session_with_null_session_factory
    {
        static Exception actual_exception;

        Because of = 
            () => actual_exception = Catch.Exception(() => new RepositorySession(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_session_factory_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("sessionFactory");
    }

    [Subject(typeof(RepositorySession))]
    class when_disposing_repository_session_without_openning_connection
    {
        static Mock<ISessionFactory> session_factory;
        static RepositorySession repository_session;

        Establish context = () =>
        {
            session_factory = new Mock<ISessionFactory>();

            repository_session = new RepositorySession(session_factory.Object);
        };

        Because of =
            () => repository_session.Dispose();

        It should_not_dispose_session_factory =
            () => session_factory.Verify(x => x.Dispose(), Times.Never());
    }

    [Subject(typeof(RepositorySession))]
    class when_getting_nhibernate_session_from_closed_db_connection_and_then_disposing_repository_session
    {
        static Mock<ISessionFactory> session_factory;
        static Mock<ISession> mock_session;
        static ISession returned_session;
        static RepositorySession repository_session;

        Establish context = () =>
        {
            mock_session = new Mock<ISession>();

            session_factory = new Mock<ISessionFactory>();
            session_factory.Setup(x => x.OpenSession()).Returns(mock_session.Object);

            repository_session = new RepositorySession(session_factory.Object);
        };

        Because of = () =>
        {
            returned_session = repository_session.NHibernateSession;
            repository_session.Dispose();
        };

        It should_return_expected_nhibernate_session =
            () => returned_session.ShouldBeTheSameAs(mock_session.Object);

        It should_not_dispose_session_factory =
            () => session_factory.Verify(x => x.Dispose(), Times.Never());

        It should_dispose_nhibernate_session =
            () => mock_session.Verify(x => x.Dispose(), Times.AtLeastOnce());
    }
}
