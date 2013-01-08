using System;
using System.Collections.Generic;
using System.Transactions;
using DocaLabs.EntityFrameworkStorage;
using DocaLabs.Storage.Core.Repositories;
using DocaLabs.Storage.Integration.Tests._Repositories._EntityFramework._Utils;
using DocaLabs.Storage.Integration.Tests._Repositories._Scenarios;
using DocaLabs.Storage.Integration.Tests._Utils;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Integration.Tests._Repositories._EntityFramework
{
    [Subject(typeof(RepositorySessionFactory))]
    class when_using_repository_session_factory
    {
        static RepositorySessionFactory repository_session_factory;
        static IRepositoryFactory second_repository_factory;
        static Book original_book;
        static Book retrieved_book;

        Cleanup after_each =
            () => second_repository_factory.Dispose();

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

            RepositoryAggregateRoot<Book>.OnModelCreatingAction = ScenarioProviderBase.OnModelCreating;

            var contextFactory = new DbContextFactory<RepositoryAggregateRoot<Book>>(MsSqlHelper.EfConnectionStringName);

            repository_session_factory = new RepositorySessionFactory(contextFactory);
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

            // cannot dispose here as the retrieved_book will be used outside context scope (which is disposed by the repository factory)
            second_repository_factory = repository_session_factory.Create();
            retrieved_book = second_repository_factory.CreateRepository<Book>().Get(original_book.Id);
        };

        It should_be_possible_to_persist_and_retrieve_an_entity =
            () => retrieved_book.ShouldMatch(original_book);
    }
}
