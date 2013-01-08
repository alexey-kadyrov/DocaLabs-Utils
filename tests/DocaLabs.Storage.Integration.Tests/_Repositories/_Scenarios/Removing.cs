using System;
using System.Collections.Generic;
using System.Transactions;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Scenarios
{
    class Removing<TScenarioPrivider> : RepositoryScenarioBase<TScenarioPrivider>
         where TScenarioPrivider : IRepositoryScenarioProvider, new()
    {
        IList<Book> OriginalBooks { get; set; }
        public Book OriginalUnchangedBook { get { return OriginalBooks[1]; } }
        public Book RemovedBook { get { return GetPersistedBook(OriginalBooks[0].Id); } }
        public Book UnchangedBook { get { return GetPersistedBook(OriginalBooks[1].Id); } }
        public bool ForceLoadPrices { get; set; }

        public Removing()
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

        public void Run()
        {
            using (var scope = new TransactionScope())
            {
                var book = Books.Get(OriginalBooks[0].Id);

                if (ForceLoadPrices)   // for the EF sake to force load prices otherwise they won't be deleted
                    Console.WriteLine(@"Loaded {0} prices", book.Prices.Count);

                Books.Remove(book);
                Books.Session.SaveChanges();    // that's optional for NHibernate
                scope.Complete();
            }
        }
    }
}
