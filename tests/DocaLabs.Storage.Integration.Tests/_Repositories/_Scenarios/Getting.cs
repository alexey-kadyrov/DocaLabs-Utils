using System;
using System.Collections.Generic;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Scenarios
{
    class Getting<TScenarioPrivider> : RepositoryScenarioBase<TScenarioPrivider>
        where TScenarioPrivider : IRepositoryScenarioProvider, new()
    {
        IList<Book> OriginalBooks { get; set; }
        public Book FirstOriginalBook { get { return OriginalBooks[0]; } }
        public Book SecondOriginalBook { get { return OriginalBooks[1]; } }
        public Book FirstFoundBook { get; private set; }
        public Book SecondFoundBook { get; private set; }

        public Getting()
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
            FirstFoundBook = Books.Get(FirstOriginalBook.Id);
            SecondFoundBook = Books.Get(SecondOriginalBook.Id);
        }
    }
}