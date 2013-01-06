using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Scenarios
{
    public class Book
    {
        public Guid Id { get; set; }
        public byte[] Version { get; set; }
        public string Isbn { get; set; }
        public string Title { get; set; }
        public virtual IList<Price> Prices { get; set; }
    }

    public class Price
    {
        public Guid Id { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public decimal Value { get; set; }
    }

    public static class EntitySpecs
    {
        public static void ShouldMatch(this Book actual, Book expected)
        {
            actual.ShouldMatch(expected, expected.Title);
        }

        public static void ShouldMatch(this Book actual, Book expected, string expectedTitle)
        {
            try
            {
                actual.ShouldNotBeNull();

                actual.Id.ShouldEqual(expected.Id);
                actual.Isbn.ShouldEqual(expected.Isbn);
                actual.Title.ShouldEqual(expectedTitle);

                if (expected.Prices == null)
                {
                    actual.ShouldBeNull();
                    return;
                }

                if (expected.Prices.Count == 0)
                {
                    actual.Prices.ShouldBeEmpty();
                    return;
                }

                actual.Prices.Count.ShouldEqual(expected.Prices.Count);

                foreach (var expectedPrice in expected.Prices)
                {
                    var actualPrice = actual.Prices.FirstOrDefault(x => x.Id == expectedPrice.Id);

                    actualPrice.ShouldNotBeNull();

                    actualPrice.ShouldMatch(expectedPrice);
                }
            }
            catch (SpecificationException e)
            {
                throw new SpecificationException("Expected the actual book matching to expected but it differs by:\n" + e.Message, e);
            }
        }

        public static void ShouldMatch(this Price actual, Price expected)
        {
            try
            {
            actual.Id.ShouldEqual(expected.Id);
            actual.Country.ShouldEqual(expected.Country);
            actual.Currency.ShouldEqual(expected.Currency);
            actual.Value.ShouldEqual(expected.Value);
            }
            catch (SpecificationException e)
            {
                throw new SpecificationException("Expected the actual price matching to expected but it differs by:\n" + e.Message, e);
            }
        }
    }
}
