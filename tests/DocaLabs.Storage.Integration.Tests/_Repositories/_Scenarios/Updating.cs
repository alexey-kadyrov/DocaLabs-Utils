using System;
using System.Collections.Generic;
using System.Transactions;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Scenarios
{
    class Updating<TScenarioPrivider> : RepositoryScenarioBase<TScenarioPrivider>
        where TScenarioPrivider : IRepositoryScenarioProvider, new()
    {
        IList<Book> OriginalBooks { get; set; }
        public Book OriginalUpdatedBook { get { return OriginalBooks[0]; } }
        public Book OriginalUnchangedBook { get { return OriginalBooks[1]; } }
        public Book UpdatedBook { get { return GetPersistedBook(OriginalBooks[0].Id); } }
        public Book UnchangedBook { get { return GetPersistedBook(OriginalBooks[1].Id); } }

        public Updating()
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
                }
            };

            ScenarioProvider.Save(OriginalBooks);
        }

        public void Run(string newTitle)
        {
            using (var scope = new TransactionScope())
            {
                var book = Books.Get(OriginalBooks[0].Id);
                book.Title = newTitle;
                Books.Session.SaveChanges();    // that's optional for NHibernate
                scope.Complete();
            }
        }
    }
}