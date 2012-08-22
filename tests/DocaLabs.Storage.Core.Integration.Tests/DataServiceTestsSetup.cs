using System;
using System.Threading;
using DocaLabs.Storage.Core.Integration.Tests.TestProviders;
using DocaLabs.Testing.Common;
using Machine.Specifications;
using Machine.Specifications.Annotations;

namespace DocaLabs.Storage.Core.Integration.Tests
{
    /// <remarks>
    /// IAssemblyContext is not reliable running under Resharper's test runner
    /// see: https://github.com/machine/machine.specifications/issues/17
    /// </remarks>
    static class DataServiceHostSetup
    {
        public static string BaseUrl
        {
            get
            {
                return string.Format(@"http://localhost:{0}/DataServiceRepository/Tests/", IntegrationPortNumbers.DataServiceHostPort);
            }
        }

        static ProductDataServiceHost ServiceHost { get; set; }

        static DataServiceHostSetup()
        {
            AppDomain.CurrentDomain.DomainUnload += HandleDomainUnload;

            Console.WriteLine(@"Starting ProductDataServiceHost.");

            ServiceHost = new ProductDataServiceHost(new Uri(BaseUrl));

            while (!ServiceHost.Active)
            {
                if(ServiceHost.Failed)
                {
                    Console.WriteLine(@"ProductDataServiceHost failed to start.");
                    return;
                }

                Thread.Sleep(100);
            }

            Console.WriteLine(@"ProductDataServiceHost started.");
        }

        static void HandleDomainUnload(object sender, EventArgs e)
        {
            ServiceHost.Cancellation.Cancel();

            ServiceHost = null;

            AppDomain.CurrentDomain.DomainUnload -= HandleDomainUnload;
        }
    }

    class DataServiceRepositoryTestsContext
    {
        protected static Product first_product;
        protected static Product second_product;
        protected static Product third_product;

        [UsedImplicitly] Cleanup after_each =
            () => ProductDataSource.UnderlyingProductCollection.Clear();

        [UsedImplicitly] Establish before_each = () =>
        {
            first_product = new Product
            {
                Id = 1,
                Name = "First Product",
                Price = 99.95M,
                ReleaseDate = new DateTime(2011, 12, 29),
                Rating = 3
            };

            second_product = new Product
            {
                Id = 2,
                Name = "Second Product",
                Price = 75.05M,
                ReleaseDate = new DateTime(2011, 12, 30),
                Rating = 5
            };

            third_product = new Product
            {
                Id = 3,
                Name = "Third Product",
                Price = 33.15M,
                ReleaseDate = new DateTime(2011, 12, 31),
                Rating = 1
            };
        };
    }
}
