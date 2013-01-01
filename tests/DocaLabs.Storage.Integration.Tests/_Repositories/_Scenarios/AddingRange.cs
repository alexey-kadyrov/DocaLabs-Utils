using System;
using System.Collections.Generic;
using System.Transactions;
using DocaLabs.Storage.Core.Repositories;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Scenarios
{
    class AddingRange<TScenarioPrivider> : RepositoryScenarioBase<TScenarioPrivider>
         where TScenarioPrivider : IRepositoryScenarioProvider, new()
    {
        public List<Book> OriginalBooks { get; set; }

        public AddingRange()
        {
            OriginalBooks = new List<Book>
            {
                new Book
                {
                    Id = Guid.NewGuid(),
                    Isbn = "1111111111111",
                    Title = "Book Title 1 Created at " + DateTime.UtcNow,
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
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Isbn = "2222222222222",
                    Title = "Book Title 2 Created at " + DateTime.UtcNow,
                    Prices = new List<Price>
                    {
                        new Price
                        {
                            Id = Guid.NewGuid(),
                            Country = "IE",
                            Currency = "EUR",
                            Value = 17.99M
                        }
                    }
                }
            };
        }

        public void RunEnumerableRange(IEnumerable<Book> books)
        {
            using (var scope = new TransactionScope())
            {
                Books.AddRange(books);
                Books.Session.SaveChanges();    // that's optional for NHibernate
                scope.Complete();
            }
        }

        public void RunParamListRange(Book b1, Book b2)
        {
            using (var scope = new TransactionScope())
            {
                Books.AddRange(b1, b2);
                Books.Session.SaveChanges();    // that's optional for NHibernate
                scope.Complete();
            }
        }
    }
}
