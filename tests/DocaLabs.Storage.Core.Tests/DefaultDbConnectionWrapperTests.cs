using System.Data.Common;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using Moq;
using Moq.Protected;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Tests
{
    [Subject(typeof(DefaultDbConnectionWrapper)), UnitTestTag]
    class when_default_db_connection_wrapper_is_disposed_it_disposes_all_allocated_resources
    {
        static Mock<DbConnection> mock_connection;
        static Mock<DbConnectionString> mock_connection_string;
        static DefaultDbConnectionWrapper wrapper;
        static DbConnection wrapped_connection;

        Establish context = () =>
        {
            mock_connection = new Mock<DbConnection>();

            mock_connection_string = new Mock<DbConnectionString>("some connection string");
            mock_connection_string.Setup(x => x.CreateDbConnection()).Returns(mock_connection.Object);

            wrapper = new DefaultDbConnectionWrapper(mock_connection_string.Object);
            wrapped_connection = wrapper.Connection;
        };

        Because of =
            () => wrapper.Dispose();

        It should_dispose_wrapped_connection =
            () => mock_connection.Protected().Verify("Dispose", Times.AtLeastOnce(), true);
    }
}
