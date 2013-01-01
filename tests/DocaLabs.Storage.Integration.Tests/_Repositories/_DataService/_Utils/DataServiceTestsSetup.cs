using System;
using System.Threading;
using DocaLabs.Testing.Common;

namespace DocaLabs.Storage.Integration.Tests._Repositories._DataService._Utils
{
    /// <remarks>
    /// IAssemblyContext is not reliable running under Resharper's test runner
    /// see: https://github.com/machine/machine.specifications/issues/17
    /// </remarks>
    static class DataServiceHostSetup
    {
        static string BaseUrl
        {
            get
            {
                return string.Format(@"http://localhost:{0}/DataServiceRepository/Tests/", IntegrationPortNumbers.DataServiceHostPort);
            }
        }

        public static Uri BaseUri
        {
            get { return new Uri(BaseUrl); }
        }


        static ProductDataServiceHost ServiceHost { get; set; }

        static DataServiceHostSetup()
        {
            AppDomain.CurrentDomain.DomainUnload += HandleDomainUnload;

            Console.WriteLine(@"Starting ProductDataServiceHost.");

            ServiceHost = new ProductDataServiceHost(BaseUri);

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
}
