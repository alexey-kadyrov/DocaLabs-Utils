using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace DocaLabs.Storage.Core.Tests._DummyProviders
{
    public class DummyDbDataReader : DbDataReader
    {
        bool Closed { get; set; }

        public override void Close()
        {
            Closed = true;
        }

        public override DataTable GetSchemaTable()
        {
            return null;
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            return false;
        }

        public override int Depth
        {
            get { return 0; }
        }

        public override bool IsClosed
        {
            get { return Closed; }
        }

        public override int RecordsAffected
        {
            get { return 0; }
        }

        public override bool GetBoolean(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override byte GetByte(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotSupportedException();
        }

        public override char GetChar(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotSupportedException();
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override short GetInt16(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override int GetInt32(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override long GetInt64(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override string GetString(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override object GetValue(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override int GetValues(object[] values)
        {
            throw new NotSupportedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override int FieldCount
        {
            get { return 0; }
        }

        public override object this[int ordinal]
        {
            get { throw new NotSupportedException(); }
        }

        public override object this[string name]
        {
            get { throw new NotSupportedException(); }
        }

        public override bool HasRows
        {
            get { return false; }
        }

        public override decimal GetDecimal(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override double GetDouble(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override float GetFloat(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override string GetName(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override int GetOrdinal(string name)
        {
            throw new NotSupportedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override Type GetFieldType(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override IEnumerator GetEnumerator()
        {
            return new DummyEnumerator();
        }

        class DummyEnumerator : IEnumerator
        {
            public bool MoveNext()
            {
                return false;
            }

            public void Reset()
            {
            }

            public object Current { get { return null; } }
        }
    }
}
