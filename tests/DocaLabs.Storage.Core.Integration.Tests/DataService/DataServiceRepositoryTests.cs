using System;
using System.Collections;
using System.Data.Services.Client;
using System.Linq;
using DocaLabs.Storage.Core.DataService;
using DocaLabs.Storage.Core.Integration.Tests.TestProviders;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Integration.Tests.DataService
{
    // ReSharper disable ReplaceWithSingleCallToFirst (as the Linq for Data Services provider doesn't support it)

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_looking_for_existing_entity_by_key : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> products;
        static Product found_product;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());
            products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of =
            () => found_product = products.Find(2);

        It should_find_matching_entity =
            () => found_product.ShouldBeSimilar(second_product, Product.Compare);
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_looking_for_non_existing_entity_by_key : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> products;
        static Product found_product;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());
            products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of =
            () => found_product = products.Find(42);

        It should_return_null =
            () => found_product.ShouldBeNull();
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_querying_repository : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> products;

        Establish context = 
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone());

        Because of = 
            () => products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));

        It should_count_all_entities =
            () => products.Count().ShouldEqual(2);

        It should_get_first_product_entity_by_its_name_and_price =
            () => products.Where(x => x.Name == "First Product").Where(x => x.Price == 99.95M).ShouldContainOnlySimilar(Product.Compare, first_product);

        It should_get_second_product_entity_by_its_name_and_rating =
            () => products.Where(x => x.Name == "Second Product" && x.Rating == 5).ShouldContainOnlySimilar(Product.Compare, second_product);

        It should_contain_all_entities =
            () => products.ShouldContainOnlySimilar(Product.Compare, first_product, second_product);
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_enumerating_repository_using_explicit_ienumerable_interface : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> products;

        Establish context =
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone());

        Because of =
            () => products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));

        It should_contain_all_entities =
            () => ((IEnumerable)products).GetEnumerator().ShouldContainOnlySimilar((x, y) => Product.Compare((Product)x, (Product)y), first_product, second_product);
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_adding_single_entity : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> products;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone());
            products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            products.Add(third_product);
            products.Unit.SaveChanges();
        };

        It should_be_persisted =
            () => ProductDataSource.UnderlyingProductCollection.ShouldContainOnlySimilar(Product.Compare, first_product, second_product, third_product);
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_adding_several_entities : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> products;

        Establish context = () =>
        {
            ProductDataSource.AddSource(second_product.Clone());
            products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            products.Add(first_product);
            products.Add(third_product);
            products.Unit.SaveChanges();
        };

        It should_be_persisted =
            () => ProductDataSource.UnderlyingProductCollection.ShouldContainOnlySimilar(Product.Compare, first_product, second_product, third_product);
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_removing_single_entity : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> products;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());
            products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            var entityToBeRemoved = products.Where(x => x.Id == 2).First();
            products.Remove(entityToBeRemoved);
            products.Unit.SaveChanges();
        };

        It should_be_removed =
            () => ProductDataSource.UnderlyingProductCollection.ShouldContainOnlySimilar(Product.Compare, first_product, third_product);
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_removing_several_entities : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> products;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());
            products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            var entityToBeRemoved2 = products.Where(x => x.Id == 2).First();
            products.Remove(entityToBeRemoved2);

            var entityToBeRemoved3 = products.Where(x => x.Id == 3).First();
            products.Remove(entityToBeRemoved3);

            products.Unit.SaveChanges();
        };

        It should_be_removed =
            () => ProductDataSource.UnderlyingProductCollection.ShouldContainOnlySimilar(Product.Compare, first_product);
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_updating_single_entity : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> products;
        
        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());
            products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            var entity = products.Where(x => x.Id == 2).First();
            entity.Name = "Updated$." + entity.Name;

            second_product.Name = entity.Name;

            products.Update(entity);

            products.Unit.SaveChanges();
        };

        It should_be_persisted =
            () => ProductDataSource.UnderlyingProductCollection.ShouldContainOnlySimilar(Product.Compare, first_product, second_product, third_product);
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_updating_several_entities : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> products;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());
            products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            var entity2 = products.Find(2);
            entity2.Name = "Updated$." + entity2.Name;
            second_product.Name = entity2.Name;
            products.Update(entity2);

            var entity3 = products.Where(x => x.Id == 3).First();
            entity3.Name = "Updated$." + entity3.Name;
            third_product.Name = entity3.Name;
            products.Update(entity3);

            products.Unit.SaveChanges();
        };

        It should_be_persisted =
            () => ProductDataSource.UnderlyingProductCollection.ShouldContainOnlySimilar(Product.Compare, first_product, second_product, third_product);
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_save_changes_default_options_are_changed_using_cast_to_data_service_context_storgae_interface
    {
        static IDataServiceRepository<Product> products;

        Establish context = () =>
        {
            products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            ((IDataServiceStorageContext)products.Unit).SaveChangesDefaultOptions = SaveChangesOptions.Batch;
        };

        It should_be_possible_to_read_them_back =
            () => ((IDataServiceStorageContext)products.Unit).SaveChangesDefaultOptions.ShouldEqual(SaveChangesOptions.Batch);
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_repository_is_newed
    {
        static IDataServiceRepository<Product> products;

        Because of = () =>
        {
            products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        It service_context_should_be_of_data_service_context_type =
            () => ((IDataServiceStorageContext)products.Unit).Context.ShouldBeOfType<DataServiceContext>();

        It table_name_should_be_products =
            () => products.TableName.ShouldEqual("Products");

        It element_type_should_be_product_type =
            () => products.ElementType.ShouldEqual(typeof(Product));
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_repository_is_newed_using_overload_constructor_with_table_name
    {
        static IDataServiceRepository<Product> products;

        Because of = () =>
        {
            products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)), "ExplicitTableName");
        };

        It should_use_that_table_name =
            () => products.TableName.ShouldEqual("ExplicitTableName");
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_repository_is_newed_using_overload_constructor_with_null_table_name : DataServiceRepositoryTestsContext
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)), null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_item_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("tableName");
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_repository_is_newed_using_overload_constructor_with_null_context_manager : DataServiceRepositoryTestsContext
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new DataServiceRepository<Product>(null, "ExplicitTableName"));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_item_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("contextManager");
    }

    [Subject(typeof(DataServiceRepository<>)), IntegrationTag]
    class when_performing_several_different_opeartions_on_repository : DataServiceRepositoryTestsContext
    {
        static IDataServiceRepository<Product> products;

        Establish context = () =>
        {
            ProductDataSource.AddSource(first_product.Clone(), second_product.Clone());
            products = new DataServiceRepository<Product>(new DataServiceStorageContextManager(new Uri(DataServiceHostSetup.BaseUrl)));
        };

        Because of = () =>
        {
            products.Add(third_product);

            products.Unit.SaveChanges();

            third_product.Name = "Updated$." + third_product.Name;

            products.Update(third_product);

            products.RemoveRange(products.Where(x => x.Id == 1));

            products.Unit.SaveChanges();
        };

        It should_be_persisted =
            () => ProductDataSource.UnderlyingProductCollection.ShouldContainOnlySimilar(Product.Compare, second_product, third_product);
    }

    // ReSharper restore ReplaceWithSingleCallToFirst
}
