using System;
using System.Data;
using System.Data.Common;
using DocaLabs.NHibernateStorage;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
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
            () => actual_exception = Catch.Exception(() => new PartitionedRepositorySession(null, new Mock<IPartitionProxy>().Object));

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

        It should_report_patition_proxy_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("partitionProxy");
    }

    [Subject(typeof(PartitionedRepositorySession))]
    class when_disposing_repository_session_without_openning_connection
    {
        static Mock<ISessionFactory> session_factory;
        static Mock<IPartitionProxy> partition_proxy;
        static PartitionedRepositorySession repository_session;

        Establish context = () =>
        {
            session_factory = new Mock<ISessionFactory>();
            partition_proxy = new Mock<IPartitionProxy>();
            repository_session = new PartitionedRepositorySession(session_factory.Object, partition_proxy.Object);
        };

        Because of =
            () => repository_session.Dispose();

        It should_not_dispose_session_factory =
            () => session_factory.Verify(x => x.Dispose(), Times.Never());

        It should_not_ask_partition_proxy_for_new_connection =
            () => partition_proxy.Verify(x => x.GetConnection(), Times.Never());
    }

    [Subject(typeof(PartitionedRepositorySession))]
    class when_getting_nhibernate_session_and_then_disposing_repository_session
    {
        static Mock<ISessionFactory> session_factory;
        static Mock<IPartitionProxy> partition_proxy;
        static Mock<IDatabaseConnection> database_connection;
        static Mock<DbConnection> db_connection;
        static Mock<ISession> mock_session;
        static ISession returned_session;
        static PartitionedRepositorySession repository_session;

        Establish context = () =>
        {
            mock_session = new Mock<ISession>();

            session_factory = new Mock<ISessionFactory>();
            session_factory.Setup(x => x.OpenSession(Moq.It.IsAny<DbConnection>())).Returns(mock_session.Object);

            db_connection = new Mock<DbConnection> { CallBase = true };
            db_connection.Setup(x => x.State).Returns(ConnectionState.Closed);

            database_connection = new Mock<IDatabaseConnection>();
            database_connection.Setup(x => x.Connection).Returns(db_connection.Object);

            partition_proxy = new Mock<IPartitionProxy>();
            partition_proxy.Setup(x => x.GetConnection()).Returns(database_connection.Object);

            repository_session = new PartitionedRepositorySession(session_factory.Object, partition_proxy.Object);
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

        It should_dispose_underlying_database_connection =
            () => database_connection.Verify(x => x.Dispose(), Times.AtLeastOnce());

        It should_dispose_nhibernate_session =
            () => mock_session.Verify(x => x.Dispose(), Times.AtLeastOnce());
    }

    [Subject(typeof(PartitionedRepositorySession))]
    class when_during_getting_nhibernate_session_and_an_excpetion_is_thrown_from_oppening_the_session
    {
        static Mock<ISessionFactory> session_factory;
        static Mock<IPartitionProxy> partition_proxy;
        static Mock<IDatabaseConnection> database_connection;
        static Mock<DbConnection> db_connection;
        static Exception actual_exception;
        static PartitionedRepositorySession repository_session;

        Establish context = () =>
        {
            session_factory = new Mock<ISessionFactory>();
            session_factory.Setup(x => x.OpenSession(Moq.It.IsAny<DbConnection>())).Throws<MarkerException>();

            db_connection = new Mock<DbConnection> { CallBase = true };
            db_connection.Setup(x => x.State).Returns(ConnectionState.Closed);

            database_connection = new Mock<IDatabaseConnection>();
            database_connection.Setup(x => x.Connection).Returns(db_connection.Object);

            partition_proxy = new Mock<IPartitionProxy>();
            partition_proxy.Setup(x => x.GetConnection()).Returns(database_connection.Object);

            repository_session = new PartitionedRepositorySession(session_factory.Object, partition_proxy.Object);
        };

        Because of = 
            () => actual_exception = Catch.Exception(() => { var s = repository_session.NHibernateSession; s.ShouldNotBeNull(); });

        It should_throw_expected_exception =
            () => actual_exception.ShouldBeOfType<MarkerException>();

        It should_not_dispose_session_factory =
            () => session_factory.Verify(x => x.Dispose(), Times.Never());

        It should_open_underlying_db_connection =
            () => db_connection.Verify(x => x.Open(), Times.Once());

        It should_dispose_database_connection =
            () => database_connection.Verify(x => x.Dispose(), Times.AtLeastOnce());

        class MarkerException : Exception
        {
        }
    }
}
