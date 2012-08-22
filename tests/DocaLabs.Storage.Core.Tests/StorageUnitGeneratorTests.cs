using System;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;

namespace DocaLabs.Storage.Core.Tests
{
    public interface ITestContext : IStorageUnit, IDisposable
    {
        void Refresh(object obj);
    }

    public class TestContext : ITestContext
    {
        public void SaveChanges()
        {
        }

        public void Dispose()
        {
        }

        public void Add(object obj)
        {
        }

        public void Remove(object obj)
        {
        }

        public void Refresh(object obj)
        {
        }
    }

    public class Customer : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CustomerCategory : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public interface ICustomerUnitWithContext : IDisposable
    {
        ITestContext Context { get; }
        IStorageSet<Customer> Customers { get; }
        IStorageSet<CustomerCategory> Categories { get; }
        void Refresh(object obj);
        void SaveChanges();
    }

    public interface ICustomerUnitWithoutContext : IDisposable
    {
        IStorageSet<Customer> Customers { get; }
        IStorageSet<CustomerCategory> Categories { get; }
        void Refresh(object obj);
        void SaveChanges();
    }

    [Subject(typeof(StorageUnitGenerator)), UnitTestTag]
    class when
    {
        static Type implementation;

        Because of =
            () => implementation = StorageUnitGenerator.GetImplementation<ICustomerUnitWithContext, TestContext>();

        It should_not_be_null =
            () => implementation.ShouldNotBeNull();
    }
}
