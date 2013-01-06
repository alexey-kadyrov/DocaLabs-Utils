using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Scenarios
{
    class Querying<TScenarioPrivider> : RepositoryScenarioBase<TScenarioPrivider>
        where TScenarioPrivider : IRepositoryScenarioProvider, new()
    {
        public IList<Book> OriginalBooks { get; set; }
        public IList<Book> FoundBooks { get; private set; }

        public Querying()
        {
            OriginalBooks = new List<Book>
            {
                new Book
                {
                    Id = Guid.NewGuid(),
                    Isbn = "1111111111111",
                    Title = "First Book Title Created at " + DateTime.UtcNow,
                    Prices = new List<Price>
                    {
                        new Price
                        {
                            Id = Guid.NewGuid(),
                            Country = "GB",
                            Currency = "GBP",
                            Value = 35.99M
                        }
                    }
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Isbn = "2222222222222",
                    Title = "Second Book Title Created at " + DateTime.UtcNow,
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
                    Isbn = "3333333333333",
                    Title = "Third Book Title Created at " + DateTime.UtcNow,
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

            ScenarioProvider.Save(OriginalBooks);
        }

        public void Run()
        {
            var id0 = OriginalBooks[0].Id;
            var id1 = OriginalBooks[1].Id;
            FoundBooks = Books.Query.Where(x => x.Id == id0 || x.Id == id1).ToList();
        }

        public Book GetOriginalBook(Guid id)
        {
            var book = OriginalBooks.FirstOrDefault(x => x.Id == id);

            if(book == null)
                throw new SpecificationException(string.Format("The book with id {0} was not expected in the original collection", id));

            return book;
        }
    }
}