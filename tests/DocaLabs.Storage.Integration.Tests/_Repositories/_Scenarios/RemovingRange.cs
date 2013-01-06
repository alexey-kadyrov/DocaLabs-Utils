using System;
using System.Collections.Generic;
using System.Transactions;
using DocaLabs.Storage.Core.Repositories;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Scenarios
{
    class RemovingRange<TScenarioPrivider> : RepositoryScenarioBase<TScenarioPrivider>
         where TScenarioPrivider : IRepositoryScenarioProvider, new()
    {
        public IList<Book> OriginalBooks { get; set; }
        public Book OriginalUnchangedBook { get { return OriginalBooks[2]; } }
        public Book UnchangedBook { get { return GetPersistedBook(OriginalBooks[2].Id); } }

        public RemovingRange()
        {
            OriginalBooks = new List<Book>
            {
                new Book
                {
                    Id = Guid.NewGuid(),
                    Isbn = "1111111111111",
                    Title = "First Book Title 1 Created at " + DateTime.UtcNow,
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
                    Title = "Second Book Title 2 Created at " + DateTime.UtcNow,
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
                    Title = "Second Book Title 3 Created at " + DateTime.UtcNow,
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

        public void RunEnumerableRange(bool bForceLoadPrices = false)
        {
            using (var scope = new TransactionScope())
            {
                var b = new List<Book>
                {
                    Books.Get(OriginalBooks[0].Id),
                    Books.Get(OriginalBooks[1].Id)
                };

                if (bForceLoadPrices)   // for the EF sake to force load prices otherwise they won't be deleted
                    Console.WriteLine(@"Loaded {0} and {1} prices", b[0].Prices.Count, b[1].Prices.Count);

                Books.RemoveRange(b);
                Books.Session.SaveChanges();    // that's optional for NHibernate
                scope.Complete();
            }
        }

        public void RunParamListRange(bool bForceLoadPrices = false)
        {
            using (var scope = new TransactionScope())
            {
                var b0 = Books.Get(OriginalBooks[0].Id);
                var b1 = Books.Get(OriginalBooks[1].Id);

                if (bForceLoadPrices)   // for the EF sake to force load prices otherwise they won't be deleted
                    Console.WriteLine(@"Loaded {0} and {1} prices", b0.Prices.Count, b1.Prices.Count);

                Books.RemoveRange(b0, b1);
                Books.Session.SaveChanges();    // that's optional for NHibernate
                scope.Complete();
            }
        }
    }
}
