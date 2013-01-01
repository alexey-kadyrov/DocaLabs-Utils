using System;
using System.Data;
using System.Data.Common;
using DocaLabs.NHibernateStorage;
using DocaLabs.Storage.Core;
using Machine.Specifications;
using Moq;
using NHibernate;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Integration.Tests._Repositories._NHibernate._Partitioned
{
    [Subject(typeof(PartitionedRepositorySession))]
    class wehen_newing_repository_session_with_null_session_factory
    {
        static Exception actual_exception;

        Because of = 
            () => actual_exception = Catch.Exception(() => new PartitionedRepositorySession(null, new Mock<IDatabaseConnection>().Object));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_session_factory_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("sessionFactory");
    }

    [Subject(typeof(PartitionedRepositorySession))]
    class wehen_newing_repository_session_with_null_database_connection
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new PartitionedRepositorySession(new Mock<ISessionFactory>().Object, null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_connection_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("connection");
    }

    [Subject(typeof(PartitionedRepositorySession))]
    class when_disposing_repository_session_without_openning_connection
    {
        static Mock<ISessionFactory> session_factory;
        static Mock<IDatabaseConnection> database_connection;
        static Mock<DbConnection> db_connection;
        static PartitionedRepositorySession repository_session;
        static bool connection_disposed;

        Establish context = () =>
        {
            session_factory = new Mock<ISessionFactory>();

            db_connection = new Mock<DbConnection> { CallBase = true };
            db_connection.Object.Disposed += (sender, args) => connection_disposed = true;

            database_connection = new Mock<IDatabaseConnection>();
            database_connection.Setup(x => x.Connection).Returns(db_connection.Object);

            repository_session = new PartitionedRepositorySession(session_factory.Object, database_connection.Object);
        };

        Because of =
            () => repository_session.Dispose();

        It should_not_dispose_session_factory =
            () => session_factory.Verify(x => x.Dispose(), Times.Never());

        It should_not_close_underlying_db_connection =
            () => db_connection.Verify(x => x.Close(), Times.Never());

        It should_not_dispose_underlying_db_connection =
            () => connection_disposed.ShouldBeFalse();

        It should_not_dispose_database_connection =
            () => database_connection.Verify(x => x.Dispose(), Times.Never());
    }

    [Subject(typeof(PartitionedRepositorySession))]
    class when_getting_nhibernate_session_from_closed_db_connection_and_then_disposing_repository_session
    {
        static Mock<ISessionFactory> session_factory;
        static Mock<IDatabaseConnection> database_connection;
        static Mock<DbConnection> db_connection;
        static Mock<ISession> mock_session;
        static ISession returned_session;
        static PartitionedRepositorySession repository_session;
        static bool connection_disposed;

        Establish context = () =>
        {
            mock_session = new Mock<ISession>();

            session_factory = new Mock<ISessionFactory>();
            session_factory.Setup(x => x.OpenSession(Moq.It.IsAny<DbConnection>())).Returns(mock_session.Object);

            db_connection = new Mock<DbConnection> { CallBase = true };
            db_connection.Object.Disposed += (sender, args) => connection_disposed = true;

            database_connection = new Mock<IDatabaseConnection>();
            database_connection.Setup(x => x.Connection).Returns(db_connection.Object);

            repository_session = new PartitionedRepositorySession(session_factory.Object, database_connection.Object);
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

        It should_open_underlying_db_connection =
            () => db_connection.Verify(x => x.Open(), Times.Once());

        It should_close_underlying_db_connection =
            () => db_connection.Verify(x => x.Close(), Times.AtLeastOnce());

        It should_dispose_nhibernate_session =
            () => mock_session.Verify(x => x.Dispose(), Times.AtLeastOnce());

        It should_not_dispose_underlying_db_connection =
            () => connection_disposed.ShouldBeFalse();

        It should_not_dispose_database_connection =
            () => database_connection.Verify(x => x.Dispose(), Times.Never());
    }

    [Subject(typeof(PartitionedRepositorySession))]
    class when_during_getting_nhibernate_session_from_closed_db_connection_an_excpetion_is_thrown_from_oppening_the_session
    {
        static Mock<ISessionFactory> session_factory;
        static Mock<IDatabaseConnection> database_connection;
        static Mock<DbConnection> db_connection;
        static Exception actual_exception;
        static PartitionedRepositorySession repository_session;
        static bool connection_disposed;

        Establish context = () =>
        {
            session_factory = new Mock<ISessionFactory>();
            session_factory.Setup(x => x.OpenSession(Moq.It.IsAny<DbConnection>())).Throws<MarkerException>();

            db_connection = new Mock<DbConnection> { CallBase = true };
            db_connection.Object.Disposed += (sender, args) => connection_disposed = true;

            database_connection = new Mock<IDatabaseConnection>();
            database_connection.Setup(x => x.Connection).Returns(db_connection.Object);

            repository_session = new PartitionedRepositorySession(session_factory.Object, database_connection.Object);
        };

        Because of = 
            () => actual_exception = Catch.Exception(() => { var s = repository_session.NHibernateSession; });

        It should_throw_expected_exception =
            () => actual_exception.ShouldBeOfType<MarkerException>();

        It should_not_dispose_session_factory =
            () => session_factory.Verify(x => x.Dispose(), Times.Never());

        It should_open_underlying_db_connection =
            () => db_connection.Verify(x => x.Open(), Times.Once());

        It should_close_underlying_db_connection =
            () => db_connection.Verify(x => x.Close(), Times.AtLeastOnce());

        It should_not_dispose_underlying_db_connection =
            () => connection_disposed.ShouldBeFalse();

        It should_not_dispose_database_connection =
            () => database_connection.Verify(x => x.Dispose(), Times.Never());

        class MarkerException : Exception
        {
        }
    }

    [Subject(typeof(PartitionedRepositorySession))]
    class when_getting_nhibernate_session_from_open_db_connection_and_then_disposing_repository_session
    {
        static Mock<ISessionFactory> session_factory;
        static Mock<IDatabaseConnection> database_connection;
        static Mock<DbConnection> db_connection;
        static Mock<ISession> mock_session;
        static ISession returned_session;
        static PartitionedRepositorySession repository_session;
        static bool connection_disposed;

        Establish context = () =>
        {
            mock_session = new Mock<ISession>();

            session_factory = new Mock<ISessionFactory>();
            session_factory.Setup(x => x.OpenSession(Moq.It.IsAny<DbConnection>())).Returns(mock_session.Object);

            db_connection = new Mock<DbConnection> { CallBase = true };
            db_connection.Setup(x => x.State).Returns(ConnectionState.Open);
            db_connection.Object.Disposed += (sender, args) => connection_disposed = true;

            database_connection = new Mock<IDatabaseConnection>();
            database_connection.Setup(x => x.Connection).Returns(db_connection.Object);

            repository_session = new PartitionedRepositorySession(session_factory.Object, database_connection.Object);
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

        It should_not_open_underlying_db_connection =
            () => db_connection.Verify(x => x.Open(), Times.Never());

        It should_not_close_underlying_db_connection =
            () => db_connection.Verify(x => x.Close(), Times.Never());

        It should_dispose_nhibernate_session =
            () => mock_session.Verify(x => x.Dispose(), Times.AtLeastOnce());

        It should_not_dispose_underlying_db_connection =
            () => connection_disposed.ShouldBeFalse();

        It should_not_dispose_database_connection =
            () => database_connection.Verify(x => x.Dispose(), Times.Never());
    }
}
