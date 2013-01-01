using Machine.Specifications;
using Microsoft.WindowsAzure.Storage.Table;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Azure._Utils
{
    public class AzureRepositoryTestProduct : TableEntity
    {
        public string Name { get; set; }
    }

    public static class EntitySpecs
    {
        public static void ShouldMatch(this AzureRepositoryTestProduct actual, AzureRepositoryTestProduct expected)
        {
            actual.ShouldMatch(expected, expected.Name);
        }

        public static void ShouldMatch(this AzureRepositoryTestProduct actual, AzureRepositoryTestProduct expected, string expectedName)
        {
            try
            {
                actual.PartitionKey.ShouldEqual(expected.PartitionKey);
                actual.RowKey.ShouldEqual(expected.RowKey);
                actual.Name.ShouldEqual(expectedName);
            }
            catch (SpecificationException e)
            {
                throw new SpecificationException("Expected the actual product matching to expected but it differs by:\n" + e.Message, e);
            }
        }
    }
}
