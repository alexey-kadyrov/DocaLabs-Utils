using System;
using System.Collections.Generic;
using System.Transactions;
using DocaLabs.NHibernateStorage;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Integration.Tests._Repositories._NHibernate._Utils;
using DocaLabs.Storage.Integration.Tests._Repositories._Scenarios;
using DocaLabs.Storage.Integration.Tests._Utils;
using Machine.Specifications;
using Moq;
using NHibernate;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Integration.Tests._Repositories._NHibernate._Partitioned
{
    [Subject(typeof(PartitionedRepositorySessionFactory))]
    class when_using_repository_session_factory
    {
        static ISessionFactory session_factory;
        static Mock<IPartitionProxy> partition_proxy;
        static PartitionedRepositorySessionFactory repository_session_factory;
        static Book original_book;
        static Book retrieved_book;

        Cleanup after_each = 
            () => session_factory.Dispose();

        Establish context = () =>
        {
            original_book = new Book
            {
                Id = Guid.NewGuid(),
                Isbn = "1234567890123",
                Title = "Book Title Created at " + DateTime.UtcNow,
                Prices = new List<Price>
                {
                    new Price
                    {
                        Id = Guid.NewGuid(),
                        Country = "GB",
                        Currency = "GBP",
                        Value = 12.99M
                    }
                }
            };

            session_factory = NHibernateSessionFactoryBuilder.Build();

            partition_proxy = new Mock<IPartitionProxy>();
            partition_proxy.Setup(x => x.GetConnection())
                           .Returns(new DatabaseConnection(new DatabaseConnectionString(MsSqlHelper.ConnectionStringSettings)));

            repository_session_factory = new PartitionedRepositorySessionFactory(session_factory, partition_proxy.Object);
        };

        Because of = () =>
        {
            using (var scope = new TransactionScope())
            using (var session = repository_session_factory.Create())
            {
                var books = session.CreateRepository<Book>();
                books.Add(original_book);
                books.Session.SaveChanges();
                scope.Complete();
            }

            using (var session = repository_session_factory.Create())
            {
                retrieved_book = session.CreateRepository<Book>().Get(original_book.Id);
            }
        };

        It should_be_possible_to_persist_and_retrieve_an_entity =
            () => retrieved_book.ShouldMatch(original_book);
    }
}
