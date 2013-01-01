using System;
using System.Collections.Generic;
using System.Transactions;
using DocaLabs.NHibernateStorage;
using DocaLabs.Storage.Integration.Tests._Repositories._NHibernate._Utils;
using DocaLabs.Storage.Integration.Tests._Repositories._Scenarios;
using Machine.Specifications;
using NHibernate;

namespace DocaLabs.Storage.Integration.Tests._Repositories._NHibernate._Ordinary
{
    [Subject(typeof(RepositorySessionFactory))]
    class when_using_repository_session_factory
    {
        static ISessionFactory session_factory; 
        static RepositorySessionFactory repository_session_factory;
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
            repository_session_factory = new RepositorySessionFactory(session_factory);
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
