using System.Data;
using System.Data.Common;

namespace DocaLabs.Storage.Core.Tests._DummyProviders
{
    public class DummyDbCommand : DbCommand
    {
        DummyDbParameterCollection ParameterCollection { get; set; }

        public override void Prepare()
        {
        }

        public override string CommandText { get; set; }

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection DbConnection { get; set; }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return ParameterCollection; }
        }

        protected override DbTransaction DbTransaction { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public DummyDbCommand()
        {
            ParameterCollection = new DummyDbParameterCollection();
        }

        public override void Cancel()
        {
        }

        protected override DbParameter CreateDbParameter()
        {
            return new DummyDbParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return new DummyDbDataReader();
        }

        public override int ExecuteNonQuery()
        {
            return 0;
        }

        public override object ExecuteScalar()
        {
            return null;
        }
    }
}
