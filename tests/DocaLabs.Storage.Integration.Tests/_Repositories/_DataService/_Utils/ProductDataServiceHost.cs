using System;
using System.Data.Services;
using System.Threading;
using System.Threading.Tasks;

namespace DocaLabs.Storage.Integration.Tests._Repositories._DataService._Utils
{
    public class ProductDataServiceHost
    {
        volatile bool _active;
        volatile bool _failed ;

        public Uri BaseAddress { get; private set; }

        public Task Task { get; private set; }

        public CancellationTokenSource Cancellation { get; private set; }

        public bool Active
        {
            get { return _active; }
            private set { _active = value; }
        }

        public bool Failed
        {
            get { return _failed; }
            private set { _failed = value; }
        }

        public ProductDataServiceHost(Uri baseAddress)
            : this(baseAddress, new CancellationTokenSource())
        {
        }

        public ProductDataServiceHost(Uri baseAddress, CancellationTokenSource cancellation)
        {
            BaseAddress = baseAddress;

            Cancellation = cancellation;

            Task = Task.Factory.StartNew(() => Listen(Cancellation.Token), TaskCreationOptions.LongRunning);
        }

        void Listen(CancellationToken cancellationToken)
        {
            using (var host = new DataServiceHost(typeof(ProductDataService), new[] { BaseAddress }))
            {
                try
                {
                    host.Open();

                    Active = true;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        Thread.Sleep(100);
                    }

                    Active = false;

                    host.Close();
                }
                catch(Exception e)
                {
                    Active = false;
                    Failed = true;
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
