using System.Data;
using System.Data.Common;

namespace DocaLabs.Storage.Core.Tests._DummyProviders
{
    public class DummyDbTransaction : DbTransaction
    {
        readonly DbConnection _connection;

        public DummyDbTransaction(DbConnection connection)
        {
            _connection = connection;
        }

        public override void Commit()
        {
        }

        public override void Rollback()
        {
        }

        protected override DbConnection DbConnection
        {
            get { return _connection; }
        }

        public override IsolationLevel IsolationLevel
        {
            get { return IsolationLevel.Serializable; }
        }
    }
}
