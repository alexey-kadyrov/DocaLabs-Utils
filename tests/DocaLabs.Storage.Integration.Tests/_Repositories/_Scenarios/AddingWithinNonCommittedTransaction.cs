using System;
using System.Collections.Generic;
using System.Transactions;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Scenarios
{
    class AddingWithinNonCommittedTransaction<TScenarioPrivider> : RepositoryScenarioBase<TScenarioPrivider>
        where TScenarioPrivider : IRepositoryScenarioProvider, new()
    {
        public Book OriginalBook { get; set; }
        public Book AddedBook { get { return GetPersistedBook(OriginalBook.Id); } }

        public AddingWithinNonCommittedTransaction()
        {
            OriginalBook = new Book
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
        }

        public void Run()
        {
            using (new TransactionScope())
            {
                Books.Add(OriginalBook);
                Books.Session.SaveChanges();    // that's optional for NHibernate
            }
        }
    }
}