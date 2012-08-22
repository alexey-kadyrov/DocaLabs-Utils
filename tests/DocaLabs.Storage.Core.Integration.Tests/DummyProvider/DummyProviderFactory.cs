using System.Data.Common;

namespace DocaLabs.Storage.Core.Integration.Tests.DummyProvider
{
    public class DummyProviderFactory : DbProviderFactory
    {
        // not documented anywhere but thanks to
        // http://ljusberg.se/blogs/smorakning/archive/2005/11/28/Custom-Data-Provider-_2800_continued_2900_.aspx
        // each factory must have this field
        public static readonly DummyProviderFactory Instance = new DummyProviderFactory();

        public override DbConnection CreateConnection()
        {
            return new DummyDbConnection();
        }
    }
}
