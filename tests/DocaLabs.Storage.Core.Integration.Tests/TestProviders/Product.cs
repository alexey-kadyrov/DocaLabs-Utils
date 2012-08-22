using System;
using System.Data.Services.Common;
using Newtonsoft.Json;

namespace DocaLabs.Storage.Core.Integration.Tests.TestProviders
{
    [DataServiceKey("Id")]
    public class Product : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Rating { get; set; }

        public Product Clone()
        {
            return new Product
            {
                Id = Id,
                Name = Name,
                Price = Price,
                ReleaseDate = ReleaseDate,
                Rating = Rating
            };
        }

        /// <remarks>
        /// Compares two instances of the Product class.
        /// It's impossible to redefine the equality behaviour as it will mess the entity tracking
        /// in the DataServiceContext which uses Dictionary and expects the default behaviour for
        /// reference types - comparing references.
        /// The dictionary uses hash code first to assign bucket and if there are more than one object 
        /// with the same hash it will compare those objects.
        /// If the equality is redefined the way that it compares properties of the object the DataServiceContext
        /// won't be able to detect that it's already tracking the entity as the dictionary will look for the
        /// changed object in different bucket.
        /// By convention if the Equals is redefined the GetHaskCode must be redefined as well in the way
        /// that it must always return the same value for equal objects.
        /// </remarks>
        public static bool Compare(Product x, Product y)
        {
            return x.Id == y.Id && x.Name == y.Name && x.Price == y.Price && x.ReleaseDate == y.ReleaseDate && x.Rating == y.Rating;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
