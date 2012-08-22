using System;
using System.Data;
using System.Data.Common;

namespace DocaLabs.Storage.Core.Integration.Tests.DummyProvider
{
    public class DummyDbConnection : DbConnection
    {
        public static Func<DbConnection, DbTransaction> TransactionFactory { get; set; }
        public static Func<DbConnection, DbCommand> CommandFactory { get; set; }

        static DummyDbConnection()
        {
            TransactionFactory = c => new DummyDbTransaction(c);
            CommandFactory = c => new DummyDbCommand { Connection = c };
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            if (TransactionFactory != null)
            {
                return TransactionFactory(this);
            }

            throw new NotSupportedException();
        }

        public override void Close()
        {
        }

         public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Open()
        {
        }

        public override string ConnectionString { get; set; }

        public override string Database
        {
            get { throw new NotSupportedException(); }
        }

        public override ConnectionState State
        {
            get { throw new NotSupportedException(); }
        }

        public override string DataSource
        {
            get { throw new NotSupportedException(); }
        }

        public override string ServerVersion
        {
            get { throw new NotSupportedException(); }
        }

        protected override DbCommand CreateDbCommand()
        {
            if (CommandFactory != null)
            {
                return CommandFactory(this);
            }

            throw new NotSupportedException();
        }
    }
}
