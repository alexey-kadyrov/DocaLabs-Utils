using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Threading.Tasks;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.MSpec;
using DocaLabs.Testing.Common.Mathematics;
using Machine.Specifications;

namespace DocaLabs.Storage.Core.Integration.Tests.Partitioning
{
    static class PartitionValidator
    {
        public static DbConnectionString[] PartitionConnectionStrings;

        static PartitionValidator()
        {
            PartitionConnectionStrings = new []
            {
                MakePartitionDbConnectionString(0),
                MakePartitionDbConnectionString(1),
                MakePartitionDbConnectionString(2),
                MakePartitionDbConnectionString(3),
                MakePartitionDbConnectionString(4),
                MakePartitionDbConnectionString(5),
                MakePartitionDbConnectionString(6)
            };
        }

        public static DbConnectionString MakePartitionDbConnectionString(int partition)
        {
            return new DbConnectionString(MakePartitionConnectionString(partition));
        }

        public static string MakePartitionConnectionString(int partition)
        {
            return string.Format(@"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsPartitionTestsPartition{0};Integrated Security=SSPI;", partition);
        }

        /// <remarks>
        /// * Validation is done by running the loop in parallel using 'Parallel.For(..'.
        /// * Keys are generated in clustered distribution using 'Guid(a,b,c,d,e,f,g,h,j,k)' by changing the 'c'.
        /// The sample size is run twice, first time in order to populate and second time to compare with the
        /// results of the first run.
        /// </remarks>
        public static DistributionStore<int> ShouldGetPartitionsForKeys(this IPartitionConnectionProvider provider, short sampleSize)
        {
            var partitionDistribution = new DistributionStore<int>();

            var partitionMap = new ConcurrentDictionary<Guid, int>();

            // should populate (most likely as there is only slight chance of clashing with keys from other specs) & return
            Parallel.For(0, sampleSize, i => 
            {
                var key = new Guid(22, 43, (short) i, 1, 2, 3, 4, 5, 6, 7, 8);
                var partition = provider.GetConnection(key).Connection.ExtractPartitionFromConnection();
                partitionMap[key] = partition;
                partitionDistribution.Add(partition);
            });

            // should return already existing
            Parallel.For(0, sampleSize, i => 
            {
                var key = new Guid(22, 43, (short) i, 1, 2, 3, 4, 5, 6, 7, 8);
                var partition = provider.GetConnection(key).Connection.ExtractPartitionFromConnection();
                partitionMap.ShouldContainKeyValue(key, partition);
            });

            return partitionDistribution;
        }

        public static int ExtractPartitionFromConnection(this DbConnection connection)
        {
            connection.ShouldNotBeNull();

            var partition = ExtractPartitionFromConnectionString(connection.ConnectionString);

            return partition;
        }

        static int ExtractPartitionFromConnectionString(string connectionString)
        {
            const string databaseNamePrefix = "DocaLabsPartitionTestsPartition";

            connectionString.ShouldContain(databaseNamePrefix);

            return Int32.Parse(
                connectionString.Substring(connectionString.IndexOf(databaseNamePrefix, StringComparison.OrdinalIgnoreCase) + databaseNamePrefix.Length, 1));
        }
    }
}
