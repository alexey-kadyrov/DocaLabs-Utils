using System.Data;
using System.Data.Common;

namespace DocaLabs.Storage.SqlAzure.Partitioning
{
    /// <summary>
    /// Executes a federation command.
    /// </summary>
    public class FederationCommand
    {
        /// <summary>
        /// Gets or sets federation command statement. The default value is: @"USE FEDERATION [{0}] ([{1}]={2}) WITH FILTERING=ON, RESET";
        /// </summary>
        /// <returns>
        /// At this point in time it's impossible to use parameterised federation command (the ExecuteNonQuery will throw an exception that you cannot use USE command to switch databases)
        /// so you should be careful with the inputs for Federation Name, Distribution Name and especially key. The federation and distribution names are usually configured and don't
        /// rely on user input but the key may come from some kind of user input.
        /// </returns>
        public static string FederationStatement { get; set; }

        /// <summary>
        /// Gets the federation name which will be used when the command is executed.
        /// </summary>
        public string FederationName { get; private set; }

        /// <summary>
        /// Gets the distribution name which will be used when the command is executed.
        /// </summary>
        public string DistributionName { get; private set; }

        /// <summary>
        /// Gets the key which will be used when the command is executed.
        /// </summary>
        public object Key { get; private set; }

        static FederationCommand()
        {
            FederationStatement = @"USE FEDERATION [{0}] ([{1}]={2}) WITH FILTERING=ON, RESET";
        }

        /// <summary>
        /// Initializes an instance of the FederationCommand.
        /// </summary>
        /// <param name="federationName">Federation name which will be used when the command is executed.</param>
        /// <param name="distributionName">Distribution name which will be used when the command is executed.</param>
        /// <param name="key">Key which will be used when the command is executed.</param>
        public FederationCommand(string federationName, string distributionName, object key)
        {
            FederationName = federationName;
            DistributionName = distributionName;
            Key = key;
        }

        /// <summary>
        /// Executes the federation command.
        /// </summary>
        /// <param name="connection">Connection for which the command will be executed.</param>
        public virtual void Execute(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = Key is string
                    ? string.Format(FederationStatement, FederationName, DistributionName, "'" + Key + "'")
                    : string.Format(FederationStatement, FederationName, DistributionName, Key);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }
    }
}
