using System;
using DocaLabs.Storage.Azure.Tables;
using DocaLabs.Storage.Core.Utils;
using Newtonsoft.Json;

namespace DocaLabs.Storage.Azure.Integration.Tests.TestProviders
{
    [TableName(TableName)]
    public class Product : AzureTableEntity
    {
        public const string TableName = "DocaLabsTestProducts";

        [Obsolete("This constructor should not be used. It is left public for compatibility with Azure Tables only. Use parametrised constructor instead.")]
        public Product()
        {
        }

        public Product(string category, Guid productId)
            : base(category, productId.ToString())
        {
        }

        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Rating { get; set; }

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
            return x.PartitionKey == y.PartitionKey && x.RowKey == y.RowKey && x.Name == y.Name 
                && Math.Abs(x.Price - y.Price) < double.Epsilon && x.ReleaseDate == y.ReleaseDate && x.Rating == y.Rating;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
