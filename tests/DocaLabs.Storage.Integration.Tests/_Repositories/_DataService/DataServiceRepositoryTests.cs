using System;
using System.Collections.Generic;
using System.Linq;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Repositories;
using DocaLabs.Storage.Core.Repositories.DataService;
using DocaLabs.Storage.Integration.Tests._Repositories._DataService._Utils;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Integration.Tests._Repositories._DataService
{
    // the simplest way to run tests with admin rights
    // see: http://blogs.msdn.com/b/amitlale/archive/2007/01/29/addressaccessdeniedexception-cause-and-solution.aspx
    // or run the command: "netsh http add urlacl url=http://+:56789/ user=DOMAIN\UserName"

    [Subject(typeof(DataServiceRepository<>))]
    class when_data_service_repository_is_newed_with_explicit_table_name
    {
        static string previous_table_name;
        static DataServiceRepository<Product> products;
        static Product original_product;

        Cleanup after_each = () =>
        {
            ProductDataSource.UnderlyingProductCollection.Clear();
            EntityToNameMap.Configure<Product>(previous_table_name);
        };

        Establish context = () =>
        {
            previous_table_name = EntityToNameMap.Get<Product>();
            EntityToNameMap.Configure<Product>("SomeStupidName");

            original_product = new Product
            {
                Id = 1,
                Name = "First Product",
                Price = 99.95M,
                ReleaseDate = new DateTime(2011, 12, 29),
                Rating = 3
            };
        };

        Because of = () =>
        {
            products = new DataServiceRepository<Product>(DataServiceHostSetup.BaseUri, "Products");

            products.Add(original_product.Clone());
            products.Session.SaveChanges();
        };

        It should_initialize_table_name_property_to_the_name_resolved_for_the_entity_type =
            () => products.TableName.ShouldEqual("Products");

        It should_use_the_provided_table_name_resolved_for_the_entity_type_for_storage_operations =
            () => data_service_repository_tests_context.GetPersistedProduct(original_product.Id).ShouldMatch(original_product);
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_data_service_repository_is_newed_without_explicit_table_name
    {
        static DataServiceRepository<Product> products;
        static Product original_product;

        Cleanup after_each =
            () => ProductDataSource.UnderlyingProductCollection.Clear();

        Establish context = () => original_product = new Product
        {
            Id = 1,
            Name = "First Product",
            Price = 99.95M,
            ReleaseDate = new DateTime(2011, 12, 29),
            Rating = 3
        };

        Because of = () =>
        {
            products = new DataServiceRepository<Product>(DataServiceHostSetup.BaseUri);

            products.Add(original_product.Clone());
            products.Session.SaveChanges();
        };

        It should_initialize_table_name_property_to_the_name_resolved_for_the_entity_type =
            () => products.TableName.ShouldEqual(EntityToNameMap.Get<Product>());

        It should_use_the_provided_table_name_resolved_for_the_entity_type_for_storage_operations =
            () => data_service_repository_tests_context.GetPersistedProduct(original_product.Id).ShouldMatch(original_product);
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_getting_entity_by_key : data_service_repository_tests_context
    {
        static Product found_product;

        Establish context = 
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());

        Because of =
            () => found_product = products.Get(second_product.Id);

        It should_find_matching_entity =
            () => found_product.ShouldMatch(second_product);
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_querying_repository : data_service_repository_tests_context
    {
        static IList<Product> found_products;

        Establish context = 
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());

        Because of =
            () => found_products = products.Query.Where(x => x.Name == "First Product").ToList();

        It should_return_expected_number_of_entities =
            () => found_products.Count().ShouldEqual(1);

        It should_find_entity =
            () => found_products[0].ShouldMatch(first_product);
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_executing_query : data_service_repository_tests_context
    {
        static IList<Product> found_products;

        Establish context =
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());

        Because of =
            () => found_products = products.Execute(new FirstProductQuery());

        It should_return_expected_number_of_entities =
            () => found_products.Count().ShouldEqual(1);

        It should_find_entity =
            () => found_products[0].ShouldMatch(first_product);

        class FirstProductQuery : DataServiceQuery<Product>
        {
            protected override IList<Product> Execute(IQueryable<Product> query)
            {
                return query.Where(x => x.Name == "First Product").ToList();
            }
        }
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_executing_a_null_query : data_service_repository_tests_context
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => products.Execute(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_query_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("query");
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_executing_scalar_query : data_service_repository_tests_context
    {
        static int number_of_products;

        Establish context =
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());

        Because of =
            () => number_of_products = products.Execute(new NumberofFirstProductsQuery());

        It should_return_expected_number_of_product =
            () => number_of_products.ShouldEqual(1);

        class NumberofFirstProductsQuery : DataServiceScalarQuery<Product, int>
        {
            // ReSharper disable RemoveToList.2
            // Count is not supported on the Data Service yet
            protected override int Execute(IQueryable<Product> query)
            {
                return query.Where(x => x.Name == "First Product").ToList().Count;
            }
            // ReSharper restore RemoveToList.2
        }
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_executing_a_null_sclar_query : data_service_repository_tests_context
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => products.Execute<int>(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_query_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("query");
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_adding_entity : data_service_repository_tests_context
    {
        Establish context = 
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone());

        Because of = () =>
        {
            products.Add(third_product.Clone());
            products.Session.SaveChanges();
        };

        It should_add_entity =
            () => GetPersistedProduct(third_product.Id).ShouldMatch(third_product);
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_adding_list_of_entities : data_service_repository_tests_context
    {
        Establish context =
            () => ProductDataSource.AddSource(third_product.Clone());

        Because of = () =>
        {
            products.AddRange(new List<Product> { second_product.Clone(), first_product.Clone() });
            products.Session.SaveChanges();
        };

        It should_add_fisrt_entity =
            () => GetPersistedProduct(first_product.Id).ShouldMatch(first_product);

        It should_add_second_entity =
            () => GetPersistedProduct(second_product.Id).ShouldMatch(second_product);
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_adding_variable_param_list_of_entities : data_service_repository_tests_context
    {
        Establish context =
            () => ProductDataSource.AddSource(third_product.Clone());

        Because of = () =>
        {
            products.AddRange(second_product.Clone(), first_product.Clone());
            products.Session.SaveChanges();
        };

        It should_add_fisrt_entity =
            () => GetPersistedProduct(first_product.Id).ShouldMatch(first_product);

        It should_add_second_entity =
            () => GetPersistedProduct(second_product.Id).ShouldMatch(second_product);
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_removing_entity : data_service_repository_tests_context
    {
        Establish context = 
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone());

        Because of = () =>
        {
            var p = products.Get(second_product.Id);
            products.Remove(p);
            products.Session.SaveChanges();
        };

        It should_delete_the_entity =
            () => GetPersistedProduct(third_product.Id).ShouldBeNull();

        It should_keep_the_entity_which_was_not_removed =
            () => GetPersistedProduct(first_product.Id).ShouldMatch(first_product);
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_removing_list_of_entities : data_service_repository_tests_context
    {
        Establish context =
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());

        Because of = () =>
        {
            var p1 = products.Get(first_product.Id);
            var p2 = products.Get(second_product.Id);
            products.RemoveRange(new List<Product> { p1, p2 });
            products.Session.SaveChanges();
        };

        It should_delete_first_entity =
            () => GetPersistedProduct(first_product.Id).ShouldBeNull();

        It should_delete_second_entity =
            () => GetPersistedProduct(second_product.Id).ShouldBeNull();

        It should_keep_the_entity_which_was_not_removed =
            () => GetPersistedProduct(third_product.Id).ShouldMatch(third_product);
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_removing_variable_param_list_of_entities : data_service_repository_tests_context
    {
        Establish context =
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());

        Because of = () =>
        {
            var p1 = products.Get(first_product.Id);
            var p2 = products.Get(second_product.Id);
            products.RemoveRange(p1, p2);
            products.Session.SaveChanges();
        };

        It should_delete_first_entity =
            () => GetPersistedProduct(first_product.Id).ShouldBeNull();

        It should_delete_second_entity =
            () => GetPersistedProduct(second_product.Id).ShouldBeNull();

        It should_keep_the_entity_which_was_not_removed =
            () => GetPersistedProduct(third_product.Id).ShouldMatch(third_product);
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_updating_entity : data_service_repository_tests_context
    {
        Establish context = 
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone());

        Because of = () =>
        {
            var p = products.Get(second_product.Id);
            p.Name = "Updated";
            products.Update(p);
            products.Session.SaveChanges();
        };

        It should_update_entity =
            () => GetPersistedProduct(second_product.Id).ShouldMatch(second_product, "Updated");

        It should_not_change_not_updated_entity =
            () => GetPersistedProduct(first_product.Id).ShouldMatch(first_product);
    }

    [Subject(typeof(DataServiceRepository<>), "replace on update")]
    class when_updating_entity_using_null_value_for_property : data_service_repository_tests_context
    {
        Establish context =
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone());

        Because of = () =>
        {
            var p = products.Get(second_product.Id);
            p.Name = null;
            products.Update(p);
            products.Session.SaveChanges();
        };

        It should_replace_on_update =
            () => GetPersistedProduct(second_product.Id).ShouldMatch(second_product, null);

        It should_not_change_not_updated_entity =
            () => GetPersistedProduct(first_product.Id).ShouldMatch(first_product);
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_updating_list_of_entities : data_service_repository_tests_context
    {
        Establish context =
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());

        Because of = () =>
        {
            var p1 = products.Get(first_product.Id);
            p1.Name = "Updated";

            var p2 = products.Get(second_product.Id);
            p2.Name = "Updated";

            products.UpdateRange(new List<Product> { p1, p2 });
            products.Session.SaveChanges();
        };

        It should_update_first_entity =
            () => GetPersistedProduct(first_product.Id).ShouldMatch(first_product, "Updated");

        It should_update_second_entity =
            () => GetPersistedProduct(second_product.Id).ShouldMatch(second_product, "Updated");

        It should_not_change_not_updated_entity =
            () => GetPersistedProduct(third_product.Id).ShouldMatch(third_product);
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_updating_variable_param_list_of_entities : data_service_repository_tests_context
    {
        Establish context =
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());

        Because of = () =>
        {
            var p1 = products.Get(first_product.Id);
            p1.Name = "Updated";

            var p2 = products.Get(second_product.Id);
            p2.Name = "Updated";

            products.UpdateRange(p1, p2);
            products.Session.SaveChanges();
        };

        It should_update_first_entity =
            () => GetPersistedProduct(first_product.Id).ShouldMatch(first_product, "Updated");

        It should_update_second_entity =
            () => GetPersistedProduct(second_product.Id).ShouldMatch(second_product, "Updated");

        It should_not_change_not_updated_entity =
            () => GetPersistedProduct(third_product.Id).ShouldMatch(third_product);
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_updating_a_null_list_of_entities : data_service_repository_tests_context
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => products.UpdateRange((IEnumerable<Product>)null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_items_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("items");
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_updating_a_null_repository_with_clist_of_entities : data_service_repository_tests_context
    {
        static Exception actual_exception;

        Establish context =
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());

        Because of = () =>
        {
            var entities = new List<Product>
            {
                products.Get(first_product.Id),
                products.Get(second_product.Id)
            };

            actual_exception = Catch.Exception(() => ((IDataServiceRepository<Product>)null).UpdateRange(entities));

            products.Session.SaveChanges();
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_repository_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("repository");
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_updating_a_null_repository_with_variable_param_list_of_entities : data_service_repository_tests_context
    {
        static Exception actual_exception;

        Establish context =
            () => ProductDataSource.AddSource(first_product.Clone(), second_product.Clone(), third_product.Clone());

        Because of = () =>
        {
            var p0 = products.Get(first_product.Id);
            var p1 = products.Get(second_product.Id);

            actual_exception = Catch.Exception(() => ((IDataServiceRepository<Product>)null).UpdateRange(p0, p1));

            products.Session.SaveChanges();
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_repository_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("repository");
    }

    [Subject(typeof(DataServiceRepository<>))]
    class when_saving_changes_in_a_batch : data_service_repository_tests_context
    {
        Establish context =
            () => ProductDataSource.AddSource(second_product.Clone(), third_product.Clone());

        Because of = () =>
        {
            products.Add(first_product.Clone());

            var p2 = products.Get(second_product.Id);
            p2.Name = "Updated";
            products.Update(p2);

            var p3 = products.Get(third_product.Id);
            products.Remove(p3);

            ((DataServiceRepositorySession)products.Session).SaveBatchChanges();
        };

        It should_add_fisrt_entity =
            () => GetPersistedProduct(first_product.Id).ShouldMatch(first_product);

        It should_update_second_entity =
            () => GetPersistedProduct(second_product.Id).ShouldMatch(second_product, "Updated");

        It should_delete_third_entity =
            () => GetPersistedProduct(third_product.Id).ShouldBeNull();
    }

    class data_service_repository_tests_context
    {
        protected static DataServiceRepository<Product> products;
        protected static Product first_product;
        protected static Product second_product;
        protected static Product third_product;

        Cleanup after_each =
            () => ProductDataSource.UnderlyingProductCollection.Clear();

        Establish before_each = () =>
        {
            first_product = new Product
            {
                Id = 1,
                Name = "First Product",
                Price = 99.95M,
                ReleaseDate = new DateTime(2011, 12, 29),
                Rating = 3
            };

            second_product = new Product
            {
                Id = 2,
                Name = "Second Product",
                Price = 75.05M,
                ReleaseDate = new DateTime(2011, 12, 30),
                Rating = 5
            };

            third_product = new Product
            {
                Id = 3,
                Name = "Third Product",
                Price = 33.15M,
                ReleaseDate = new DateTime(2011, 12, 31),
                Rating = 1
            };

            products = new DataServiceRepository<Product>(DataServiceHostSetup.BaseUri);
        };

        public static Product GetPersistedProduct(int id)
        {
            return ProductDataSource.UnderlyingProductCollection.FirstOrDefault(x => x.Id == id);
        }
    }
}
