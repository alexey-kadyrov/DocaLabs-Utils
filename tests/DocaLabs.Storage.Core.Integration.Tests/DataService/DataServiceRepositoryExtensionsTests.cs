using System;
using System.Collections.Generic;
using System.Linq;
using DocaLabs.Storage.Core.DataService;
using DocaLabs.Storage.Core.Integration.Tests.TestProviders;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;

namespace DocaLabs.Storage.Core.Integration.Tests.DataService
{
    // ReSharper disable ReplaceWithSingleCallToFirst (as the Linq for Data Services provider doesn't support it)

    [Subject(typeof(DataServiceRepositoryExtensions)), IntegrationTag]
    class when_updating_range_of_entities_in_repository_using_extension_method_with_enumerable : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());
            repository = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            var entity1 = repository.Where(x => x.Id == 1).First();
            entity1.Name = "Updated$." + entity1.Name;

            first_product.Name = entity1.Name;

            var entity3 = repository.Where(x => x.Id == 3).First();
            entity3.Name = "Updated$." + entity3.Name;

            third_product.Name = entity3.Name;

            repository.UpdateRange(new[] { entity1, entity3 });

            repository.DataService.SaveBatchChanges();
        };

        It should_be_persisted =
            () => ProductDataSource.UnderlyingProductCollection.ShouldContainOnlySimilar(Product.Compare, first_product, second_product, third_product);
    }

    [Subject(typeof(DataServiceRepositoryExtensions)), IntegrationTag]
    class when_updating_range_of_entities_in_repository_using_extension_method_with_params : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());
            repository = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            var entity1 = repository.Where(x => x.Id == 1).First();
            entity1.Name = "Updated$." + entity1.Name;

            first_product.Name = entity1.Name;

            var entity3 = repository.Where(x => x.Id == 3).First();
            entity3.Name = "Updated$." + entity3.Name;

            third_product.Name = entity3.Name;

            repository.UpdateRange(entity1, entity3);

            repository.DataService.SaveBatchChanges();
        };

        It should_be_persisted =
            () => ProductDataSource.UnderlyingProductCollection.ShouldContainOnlySimilar(Product.Compare, first_product, second_product, third_product);
    }

    [Subject(typeof(DataServiceRepositoryExtensions)), IntegrationTag]
    class when_updating_null_range_of_entities_to_repository_using_extensions_method_with_enumerable : DataServiceRepositoryTestsContext
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
            actual_exception = Catch.Exception(() => repository.UpdateRange((IEnumerable<Product>)null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_items_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("items");
    }

    [Subject(typeof(DataServiceRepositoryExtensions)), IntegrationTag]
    class when_updating_null_range_of_entities_to_repository_using_extensions_method_with_params : DataServiceRepositoryTestsContext
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
            actual_exception = Catch.Exception(() => repository.UpdateRange((Product[])null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_items_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("items");
    }

    // ReSharper restore ReplaceWithSingleCallToFirst
}
