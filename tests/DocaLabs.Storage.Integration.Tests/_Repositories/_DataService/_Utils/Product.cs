using System;
using System.Data.Services.Common;
using Machine.Specifications;

namespace DocaLabs.Storage.Integration.Tests._Repositories._DataService._Utils
{
    [DataServiceKey("Id")]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Rating { get; set; }
    }

    public static class EntitySpecs
    {
        public static Product Clone(this Product source)
        {
            return new Product
            {
                Id = source.Id,
                Name = source.Name,
                Price = source.Price,
                ReleaseDate = source.ReleaseDate,
                Rating = source.Rating
            };
        }

        public static void ShouldMatch(this Product actual, Product expected)
        {
            actual.ShouldMatch(expected, expected.Name);
        }

        public static void ShouldMatch(this Product actual, Product expected, string expectedName)
        {
            try
            {
                actual.Id.ShouldEqual(expected.Id);
                actual.Name.ShouldEqual(expectedName);
                actual.Price.ShouldEqual(expected.Price);
                actual.ReleaseDate.ShouldEqual(expected.ReleaseDate);
                actual.Rating.ShouldEqual(expected.Rating);
            }
            catch (SpecificationException e)
            {
                throw new SpecificationException("Expected the actual product matching to expected but it differs by:\n" + e.Message, e);
            }
        }
    }
}
