using System.Data.Services;
using System.Data.Services.Common;

namespace DocaLabs.Storage.Core.Integration.Tests.TestProviders
{
    public class ProductDataService : DataService<ProductDataSource>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
            config.UseVerboseErrors = true;
        }
    }
}
