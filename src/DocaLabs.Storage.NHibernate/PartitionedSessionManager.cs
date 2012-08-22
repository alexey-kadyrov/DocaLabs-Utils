using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Partitioning;
using NHibernate;

namespace DocaLabs.Storage.NHibernate
{
    /// <summary>
    /// Manages the NHibrnate sessions using CurrentPartitionProxy to get the connection.
    /// </summary>
    /// <remarks>
    /// It's possible to use DefaultSessionManager by passing CurrentPartitionProxy.Current.GetConnection()
    /// into constructor if the partition can be determined at the injection time. 
    /// The PartitionedSessionManager class however let to determine the partition much later in the
    /// lifecycle thus enabling scenarios where it's impossible to infer the partition on earlier stages.
    /// </remarks>
    public class PartitionedSessionManager : SessionManagerBase
    {
        /// <summary>
        /// Initializes an instance of the PartitionedSessionManager class with specified session factory.
        /// </summary>
        public PartitionedSessionManager(ISessionFactory sessionFactory) 
            : base(sessionFactory)
        {
        }

        /// <summary>
        /// Gets a new instance of the connection wrapper in order to open the session.
        /// </summary>
        protected override IDbConnectionWrapper GetConnection()
        {
            return CurrentPartitionProxy.Current.GetConnection();
        }
    }
}
