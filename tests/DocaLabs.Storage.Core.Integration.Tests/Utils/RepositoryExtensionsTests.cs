using System;
using System.Collections.Generic;
using System.Linq;
using DocaLabs.Storage.Core.DataService;
using DocaLabs.Storage.Core.Integration.Tests.TestProviders;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;

namespace DocaLabs.Storage.Core.Integration.Tests.Utils
{
    // ReSharper disable ReplaceWithSingleCallToFirst (as the Linq for Data Services provider doesn't support it)

    [Subject(typeof(DataServiceRepositoryExtensions)), IntegrationTag]
    class when_adding_range_of_entities_to_repository_using_extension_method_with_params : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone());
            repository = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            repository.AddRange(second_product, third_product);
            repository.DataService.SaveBatchChanges();
        };

        It should_be_persisted =
            () => ProductDataSource.UnderlyingProductCollection.ShouldContainOnlySimilar(Product.Compare, first_product, second_product, third_product);
    }

    [Subject(typeof(DataServiceRepositoryExtensions)), IntegrationTag]
    class when_adding_range_of_entities_to_repository_using_extension_method_with_enumerable : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone());
            repository = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            repository.AddRange(new[] { second_product, third_product });
            repository.DataService.SaveBatchChanges();
        };

        It should_be_persisted =
            () => ProductDataSource.UnderlyingProductCollection.ShouldContainOnlySimilar(Product.Compare, first_product, second_product, third_product);
    }

    [Subject(typeof(DataServiceRepositoryExtensions)), IntegrationTag]
    class when_adding_null_range_of_entities_to_repository_using_extension_method_with_enumerable : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;
        static Exception actual_exception;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone());
            repository = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => repository.AddRange((IEnumerable<Product>)null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_items_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("items");
    }

    [Subject(typeof(DataServiceRepositoryExtensions)), IntegrationTag]
    class when_adding_null_range_of_entities_to_repository_using_extension_method_with_params : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;
        static Exception actual_exception;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone());
            repository = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => repository.AddRange((Product[])null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_items_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("items");
    }

    [Subject(typeof(DataServiceRepositoryExtensions)), IntegrationTag]
    class when_removing_range_of_entities_in_repository_using_extension_method_with_enumerable : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());
            repository = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            repository.RemoveRange(repository.Where(x => x.Id == 1 || x.Id == 3));
            repository.DataService.SaveBatchChanges();
        };

        It should_be_removed =
            () => ProductDataSource.UnderlyingProductCollection.ShouldContainOnlySimilar(Product.Compare, second_product);
    }

    [Subject(typeof(DataServiceRepositoryExtensions)), IntegrationTag]
    class when_removing_range_of_entities_in_repository_using_extension_method_with_params : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());
            repository = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            repository.RemoveRange(repository.Where(x => x.Id == 1 || x.Id == 3).ToArray());
            repository.DataService.SaveBatchChanges();
        };

        It should_be_removed =
            () => ProductDataSource.UnderlyingProductCollection.ShouldContainOnlySimilar(Product.Compare, second_product);
    }

    [Subject(typeof(DataServiceRepositoryExtensions)), IntegrationTag]
    class when_removing_null_range_of_entities_in_repository_using_extension_method_with_enumerable : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;
        static Exception actual_exception;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone());
            repository = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => repository.RemoveRange((IEnumerable<Product>)null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_items_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("items");
    }

    [Subject(typeof(DataServiceRepositoryExtensions)), IntegrationTag]
    class when_removing_null_range_of_entities_in_repository_using_extension_method_with_params : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;
        static Exception actual_exception;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone());
            repository = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => repository.RemoveRange((Product[])null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_items_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("items");
    }

    // ReSharper restore ReplaceWithSingleCallToFirst
}
