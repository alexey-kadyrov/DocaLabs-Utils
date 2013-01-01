using System;
using System.Collections;
using System.Data.Common;
using System.Linq;

namespace DocaLabs.Storage.Core.Tests._DummyProviders
{
    public class DummyDbParameterCollection : DbParameterCollection
    {
        // ReSharper disable AssignNullToNotNullAttribute

        ArrayList Parameters { get; set; }

        public DummyDbParameterCollection()
        {
            Parameters = new ArrayList();
        }

        public override int Add(object value)
        {
            return Parameters.Add(value);
        }

        public override bool Contains(object value)
        {
            return Parameters.Contains(value);
        }

        public override void Clear()
        {
            Parameters.Clear();
        }

        public override int IndexOf(object value)
        {
            return Parameters.IndexOf(value);
        }

        public override void Insert(int index, object value)
        {
            Parameters.Insert(index, value);
        }

        public override void Remove(object value)
        {
            Parameters.Remove(value);
        }

        public override void RemoveAt(int index)
        {
            Parameters.RemoveAt(index);
        }

        public override void RemoveAt(string parameterName)
        {
            var item = Parameters.Cast<DbParameter>().SingleOrDefault(x => x.ParameterName == parameterName);
            if(item != null)
                Remove(item);
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            Parameters[index] = value;
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var item = Parameters.Cast<DbParameter>().SingleOrDefault(x => x.ParameterName == parameterName);
            if (item != null)
                Parameters[IndexOf(item)] = value;
            else
                Add(value);
        }

        public override int Count
        {
            get { return Parameters.Count; }
        }

        public override object SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        public override bool IsFixedSize
        {
            get { return false; }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSynchronized
        {
            get { return false; }
        }

        public override int IndexOf(string parameterName)
        {
            var item = Parameters.Cast<DbParameter>().SingleOrDefault(x => x.ParameterName == parameterName);
            return item != null ? IndexOf(item) : -1;
        }

        public override IEnumerator GetEnumerator()
        {
            return Parameters.GetEnumerator();
        }

        protected override DbParameter GetParameter(int index)
        {
            return Parameters[index] as DbParameter;
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            return Parameters.Cast<DbParameter>().SingleOrDefault(x => x.ParameterName == parameterName);
        }

        public override bool Contains(string value)
        {
            return Parameters.Cast<DbParameter>().SingleOrDefault(x => x.ParameterName == value) != null;
        }

        public override void CopyTo(Array array, int index)
        {
            Parameters.CopyTo(array, index);
        }

        public override void AddRange(Array values)
        {
            Parameters.AddRange(values);
        }

        // ReSharper restore AssignNullToNotNullAttribute
    }
}
