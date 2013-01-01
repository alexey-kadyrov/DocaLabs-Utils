using System;
using System.Collections.Generic;
using System.Linq;
using DocaLabs.AzureStorage.Tables;
using DocaLabs.Storage.Core;
using DocaLabs.Storage.Core.Repositories;
using DocaLabs.Storage.Integration.Tests._Repositories._Azure._Utils;
using Machine.Specifications;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Azure
{
    [Subject(typeof(AzureTableRepository<>))]
    class when_newing_the_azure_table_repository_with_explicit_table_name
    {
        const string explicit_table_name = "AzureRepositoryExplicitNameTestProducts";
        static AzureRepositoryTestProduct original_product;
        static AzureTableRepository<AzureRepositoryTestProduct> products;

        Cleanup after_each =
            () => repository_test_context.DropTable(explicit_table_name);

        Establish context = () =>
        {
            original_product = new AzureRepositoryTestProduct
            {
                PartitionKey = "Green",
                RowKey = Guid.NewGuid().ToString(),
                Name = "A Green Product"
            };

            repository_test_context.CreateTable(explicit_table_name);
        };

        Because of = () =>
        {
            products = new AzureTableRepository<AzureRepositoryTestProduct>(
                CloudStorageAccount.DevelopmentStorageAccount.CreateCloudTableClient(), explicit_table_name);

            products.Add(original_product);
            products.Session.SaveChanges();
        };

        It should_initialize_auto_save_after_number_operations_property_to_zero =
            () => ((AzureTableRepositorySession)products.Session).AutoSaveAfterNumberOperations.ShouldEqual(0);

        It should_initialize_table_name_property_to_provided_table_name =
            () => products.TableName.ShouldEqual(explicit_table_name);

        It should_use_the_provided_table_name_for_storage_operations =
            () => repository_test_context.GetProductFor(original_product, explicit_table_name).ShouldMatch(original_product);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_newing_the_azure_table_repository_without_explicit_table_name
    {
        static AzureRepositoryTestProduct original_product;
        static AzureTableRepository<AzureRepositoryTestProduct> products;

        Cleanup after_each =
            () => repository_test_context.DropTable();

        Establish context = () =>
        {
            original_product = new AzureRepositoryTestProduct
            {
                PartitionKey = "Green",
                RowKey = Guid.NewGuid().ToString(),
                Name = "A Green Product"
            };

            repository_test_context.CreateTable();
        };

        Because of = () =>
        {
            products = new AzureTableRepository<AzureRepositoryTestProduct>(CloudStorageAccount.DevelopmentStorageAccount.CreateCloudTableClient());

            products.Add(original_product);
            products.Session.SaveChanges();
        };

        It should_initialize_auto_save_after_number_operations_property_to_zero =
            () => ((AzureTableRepositorySession)products.Session).AutoSaveAfterNumberOperations.ShouldEqual(0);

        It should_initialize_table_name_property_to_the_name_resolved_for_the_entity_type =
            () => products.TableName.ShouldEqual(EntityToNameMap.Get<AzureRepositoryTestProduct>());

        It should_use_the_provided_table_name_resolved_for_the_entity_type_for_storage_operations =
            () => repository_test_context.GetProductFor(original_product, EntityToNameMap.Get<AzureRepositoryTestProduct>()).ShouldMatch(original_product);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_saving_changes_without_performing_any_operation : repository_test_context
    {
        static Exception actual_exception;

        Because of = 
            () => actual_exception = Catch.Exception(() => products.Session.SaveChanges());

        It no_exception_is_thrown =
            () => actual_exception.ShouldBeNull();
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_adding_an_entity : repository_test_context
    {
        static AzureRepositoryTestProduct original_product;

        Establish context = () =>
        {
            original_product = new AzureRepositoryTestProduct
            {
                PartitionKey = "Green",
                RowKey = Guid.NewGuid().ToString(),
                Name = "A Green Product"
            };
        };

        Because of = () =>
        {
            products.Add(original_product);
            products.Session.SaveChanges();
        };

        It should_add_the_entity =
            () => GetProductFor(original_product).ShouldMatch(original_product);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_adding_an_entity_which_already_exists : repository_test_context
    {
        static AzureRepositoryTestProduct original_product;

        Establish context = () =>
        {
            original_product = new AzureRepositoryTestProduct
            {
                PartitionKey = "Green",
                RowKey = Guid.NewGuid().ToString(),
                Name = "A Green Product"
            };

            Save(original_product);
        };

        Because of = () =>
        {
            var product = new AzureRepositoryTestProduct
            {
                PartitionKey = original_product.PartitionKey,
                RowKey = original_product.RowKey,
                Name = "Some New Name"
            };
            products.Add(product);
            products.Session.SaveChanges();
        };

        It should_add_the_entity =
            () => GetProductFor(original_product).ShouldMatch(original_product, "Some New Name");
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_adding_list_of_entities : repository_test_context
    {
        static List<AzureRepositoryTestProduct> original_products;

        Establish context = () =>
        {
            original_products = new List<AzureRepositoryTestProduct>
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                }
            };
        };

        Because of = () =>
        {
            products.AddRange(original_products);
            products.Session.SaveChanges();
        };

        It should_add_first_entity =
            () => GetProductFor(original_products[0]).ShouldMatch(original_products[0]);

        It should_add_second_entity =
            () => GetProductFor(original_products[1]).ShouldMatch(original_products[1]);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_adding_variable_params_list_of_entities : repository_test_context
    {
        static List<AzureRepositoryTestProduct> original_products;

        Establish context = () =>
        {
            original_products = new List<AzureRepositoryTestProduct>
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                }
            };
        };

        Because of = () =>
        {
            products.AddRange(original_products[0], original_products[1]);
            products.Session.SaveChanges();
        };

        It should_add_first_entity =
            () => GetProductFor(original_products[0]).ShouldMatch(original_products[0]);

        It should_add_second_entity =
            () => GetProductFor(original_products[1]).ShouldMatch(original_products[1]);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_removing_an_entity : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 2"
                }
            };

            Save(original_products);
        };

        Because of = () =>
        {
            products.Remove(original_products[0]);
            products.Session.SaveChanges();
        };

        It should_delete_entity =
            () => GetProductFor(original_products[0]).ShouldBeNull();

        It should_keep_the_entity_which_was_not_removed =
            () => GetProductFor(original_products[1]).ShouldMatch(original_products[1]);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_removing_list_of_entities : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 2"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 3"
                }
            };

            Save(original_products);
        };

        Because of = () =>
        {
            products.RemoveRange(new List<AzureRepositoryTestProduct> { original_products[0], original_products[1] });
            products.Session.SaveChanges();
        };

        It should_delete_first_entity =
            () => GetProductFor(original_products[0]).ShouldBeNull();

        It should_delete_second_entity =
            () => GetProductFor(original_products[1]).ShouldBeNull();

        It should_keep_the_entity_which_was_not_removed =
            () => GetProductFor(original_products[2]).ShouldMatch(original_products[2]);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_removing_variable_param_list_of_entities : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 2"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 3"
                }
            };

            Save(original_products);
        };

        Because of = () =>
        {
            products.RemoveRange(original_products[0], original_products[1]);
            products.Session.SaveChanges();
        };

        It should_delete_first_entity =
            () => GetProductFor(original_products[0]).ShouldBeNull();

        It should_delete_second_entity =
            () => GetProductFor(original_products[1]).ShouldBeNull();

        It should_keep_the_entity_which_was_not_removed =
            () => GetProductFor(original_products[2]).ShouldMatch(original_products[2]);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_updating_an_entity : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 2"
                }
            };

            Save(original_products);
        };

        Because of = () =>
        {
            var product = products.Get(original_products[0].PartitionKey, original_products[0].RowKey);
            product.Name = "Updated Name";
            products.Update(product);
            products.Session.SaveChanges();
        };

        It should_update_the_affected_entity =
            () => GetProductFor(original_products[0]).ShouldMatch(original_products[0], "Updated Name");

        It should_not_change_the_entity_which_was_not_changed =
            () => GetProductFor(original_products[1]).ShouldMatch(original_products[1]);
    }

    [Subject(typeof(AzureTableRepository<>), "replace on update")]
    class when_updating_entity_using_null_value_for_property : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 2"
                }
            };

            Save(original_products);
        };

        Because of = () =>
        {
            var product = products.Get(original_products[0].PartitionKey, original_products[0].RowKey);
            product.Name = null;
            products.Update(product);
            products.Session.SaveChanges();
        };

        It should_update_the_affected_entity =
            () => GetProductFor(original_products[0]).ShouldMatch(original_products[0], null);

        It should_not_change_the_entity_which_was_not_changed =
            () => GetProductFor(original_products[1]).ShouldMatch(original_products[1]);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_updating_a_collection_of_entities : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 2"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 3"
                }
            };

            Save(original_products);
        };

        Because of = () =>
        {
            var entities = new List<AzureRepositoryTestProduct>
            {
                products.Get(original_products[0].PartitionKey, original_products[0].RowKey),
                products.Get(original_products[1].PartitionKey, original_products[1].RowKey)
            };

            entities[0].Name = "Updated Name 1";
            entities[1].Name = "Updated Name 2";

            products.UpdateRange(entities);

            products.Session.SaveChanges();
        };

        It should_update_the_first_entity =
            () => GetProductFor(original_products[0]).ShouldMatch(original_products[0], "Updated Name 1");

        It should_update_the_second_entity =
            () => GetProductFor(original_products[1]).ShouldMatch(original_products[1], "Updated Name 2");

        It should_not_change_the_entity_which_was_not_updated =
            () => GetProductFor(original_products[2]).ShouldMatch(original_products[2]);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_updating_a_variable_param_list_of_entities : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 2"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 3"
                }
            };

            Save(original_products);
        };

        Because of = () =>
        {
            var p0 = products.Get(original_products[0].PartitionKey, original_products[0].RowKey);
            var p1 = products.Get(original_products[1].PartitionKey, original_products[1].RowKey);

            p0.Name = "Updated Name 1";
            p1.Name = "Updated Name 2";

            products.UpdateRange(p0, p1);

            products.Session.SaveChanges();
        };

        It should_update_the_first_entity =
            () => GetProductFor(original_products[0]).ShouldMatch(original_products[0], "Updated Name 1");

        It should_update_the_second_entity =
            () => GetProductFor(original_products[1]).ShouldMatch(original_products[1], "Updated Name 2");

        It should_not_change_the_entity_which_was_not_updated =
            () => GetProductFor(original_products[2]).ShouldMatch(original_products[2]);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_updating_a_null_collection_of_entities : repository_test_context
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => products.UpdateRange((IEnumerable<AzureRepositoryTestProduct>)null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_items_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("items");
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_updating_a_null_repository_with_collection_of_entities : repository_test_context
    {
        static Exception actual_exception;
        static AzureRepositoryTestProduct[] original_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 2"
                }
            };

            Save(original_products);
        };

        Because of = () =>
        {
            var entities = new List<AzureRepositoryTestProduct>
            {
                products.Get(original_products[0].PartitionKey, original_products[0].RowKey),
                products.Get(original_products[1].PartitionKey, original_products[1].RowKey)
            };

            actual_exception = Catch.Exception(() => ((IAzureTableRepository<AzureRepositoryTestProduct>)null).UpdateRange(entities));

            products.Session.SaveChanges();
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_repository_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("repository");
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_updating_a_null_repository_with_variable_param_list_of_entities : repository_test_context
    {
        static Exception actual_exception;
        static AzureRepositoryTestProduct[] original_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 2"
                }
            };

            Save(original_products);
        };

        Because of = () =>
        {
            var p0 = products.Get(original_products[0].PartitionKey, original_products[0].RowKey);
            var p1 = products.Get(original_products[1].PartitionKey, original_products[1].RowKey);

            actual_exception = Catch.Exception(() => ((IAzureTableRepository<AzureRepositoryTestProduct>)null).UpdateRange(p0, p1));

            products.Session.SaveChanges();
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_repository_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("repository");
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_getting_entities_by_partition_and_row_keys : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;
        static AzureRepositoryTestProduct first_found_product;
        static AzureRepositoryTestProduct second_found_product;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 2"
                }
            };

            Save(original_products);
        };

        Because of = () =>
        {
            first_found_product = products.Get(original_products[0].PartitionKey, original_products[0].RowKey);
            second_found_product = products.Get(original_products[1].PartitionKey, original_products[1].RowKey);
        };

        It should_get_the_first_entity =
            () => first_found_product.ShouldMatch(original_products[0]);

        It should_get_the_second_entity =
            () => second_found_product.ShouldMatch(original_products[1]);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_getting_an_entity_by_null_collection_of_keys : repository_test_context
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => products.Get(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_key_values_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("keyValues");
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_getting_an_entity_by_only_one_key : repository_test_context
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => products.Get("Green"));

        It should_throw_argument_exception =
            () => actual_exception.ShouldBeOfType<ArgumentException>();

        It should_report_key_values_argument =
            () => ((ArgumentException)actual_exception).ParamName.ShouldEqual("keyValues");
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_getting_an_entity_by_more_than_two_kyes : repository_test_context
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => products.Get("Green", Guid.NewGuid(), "Third Key"));

        It should_throw_argument_exception =
            () => actual_exception.ShouldBeOfType<ArgumentException>();

        It should_report_key_values_argument =
            () => ((ArgumentException)actual_exception).ParamName.ShouldEqual("keyValues");
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_getting_an_entity_by_null_partition_and_row_keys : repository_test_context
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => products.Get(null, null));

        It should_throw_argument_exception =
            () => actual_exception.ShouldBeOfType<ArgumentException>();

        It should_report_key_values_argument =
            () => ((ArgumentException)actual_exception).ParamName.ShouldEqual("keyValues");
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_getting_an_entity_by_null_partition_key : repository_test_context
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => products.Get(null, Guid.NewGuid()));

        It should_throw_argument_exception =
            () => actual_exception.ShouldBeOfType<ArgumentException>();

        It should_report_key_values_argument =
            () => ((ArgumentException)actual_exception).ParamName.ShouldEqual("keyValues");
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_getting_an_entity_by_null_row_key : repository_test_context
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => products.Get("Green", null));

        It should_throw_argument_exception =
            () => actual_exception.ShouldBeOfType<ArgumentException>();

        It should_report_key_values_argument =
            () => ((ArgumentException)actual_exception).ParamName.ShouldEqual("keyValues");
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_executing_a_query : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;
        static IList<AzureRepositoryTestProduct> found_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 1"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Blue",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Blue Product 2"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product 3"
                }
            };

            // products have different partition keys, so no batch operation there
            Save(original_products[0]);
            Save(original_products[1]);
            Save(original_products[2]);
        };

        Because of = 
            () => found_products = products.Execute(new TestQuery());

        It should_return_expected_number_of_entities =
            () => found_products.Count.ShouldEqual(1);

        It should_find_the_entity =
            () => found_products[0].ShouldMatch(original_products[1]);

        class TestQuery : AzureQuery<AzureRepositoryTestProduct>
        {
            protected override IList<AzureRepositoryTestProduct> Execute(AzureTableRepositorySession session)
            {
                var query = new TableQuery<AzureRepositoryTestProduct>().Where(
                    TableQuery.GenerateFilterCondition("Name", QueryComparisons.Equal, "A Blue Product 2"));

                return session.Table.ExecuteQuery(query).ToList();
            }
        }
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_executing_a_null_query : repository_test_context
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => products.Execute(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_query_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("query");
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_newing_repository_with_null_session : repository_test_context
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new AzureTableRepository<AzureRepositoryTestProduct>(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_table_client_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("tableClient");
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_setting_operation_context : repository_test_context
    {
        static AzureRepositoryTestProduct original_product;
        static bool operation_completed_handler_was_called;

        Establish context = () =>
        {
            original_product = new AzureRepositoryTestProduct
            {
                PartitionKey = "Green",
                RowKey = Guid.NewGuid().ToString(),
                Name = "A Green Product"
            };
        };

        Because of = () =>
        {
            ((AzureTableRepositorySession)products.Session).OperationContext = new OperationContext();
            ((AzureTableRepositorySession)products.Session).OperationContext.ResponseReceived += (sender, args) => operation_completed_handler_was_called = true;

            products.Add(original_product);
            products.Session.SaveChanges();
        };

        It should_execute_an_operation =
            () => GetProductFor(original_product).ShouldMatch(original_product);

        It should_use_the_provided_operation_context =
            () => operation_completed_handler_was_called.ShouldBeTrue();
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_adding_only_with_auto_save_after_number_operations_set_to_3 : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                }
            };
        };

        Because of = () =>
        {
            ((AzureTableRepositorySession)products.Session).AutoSaveAfterNumberOperations = 3;

            products.Add(original_products[0]);
            products.Add(original_products[1]);
            products.Add(original_products[2]);

            products.Add(original_products[3]);
        };

        It should_execute_first_operation =
            () => GetProductFor(original_products[0]).ShouldMatch(original_products[0]);

        It should_execute_second_operation =
            () => GetProductFor(original_products[1]).ShouldMatch(original_products[1]);

        It should_execute_third_operation =
            () => GetProductFor(original_products[2]).ShouldMatch(original_products[2]);

        It should_not_execute_fourth_operation_as_the_save_changes_was_not_called_for_it =
            () => GetProductFor(original_products[3]).ShouldBeNull();
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_updating_only_with_auto_save_after_number_operations_set_to_3 : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                }
            };

            Save(original_products);
        };

        Because of = () =>
        {
            ((AzureTableRepositorySession)products.Session).AutoSaveAfterNumberOperations = 3;

            var p0 = products.Get(original_products[0].PartitionKey, original_products[0].RowKey);
            var p1 = products.Get(original_products[1].PartitionKey, original_products[1].RowKey);
            var p2 = products.Get(original_products[2].PartitionKey, original_products[2].RowKey);
            var p3 = products.Get(original_products[3].PartitionKey, original_products[3].RowKey);

            p0.Name = "Changed 1";
            p1.Name = "Changed 2";
            p2.Name = "Changed 3";
            p3.Name = "Changed 4";

            products.Update(p0);
            products.Update(p1);
            products.Update(p2);

            products.Update(p3);
        };

        It should_execute_first_operation =
            () => GetProductFor(original_products[0]).ShouldMatch(original_products[0], "Changed 1");

        It should_execute_second_operation =
            () => GetProductFor(original_products[1]).ShouldMatch(original_products[1], "Changed 2");

        It should_execute_third_operation =
            () => GetProductFor(original_products[2]).ShouldMatch(original_products[2], "Changed 3");

        It should_not_execute_fourth_operation_as_the_save_changes_was_not_called_for_it =
            () => GetProductFor(original_products[3]).ShouldMatch(original_products[3]);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_removing_only_with_auto_save_after_number_operations_set_to_3 : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                }
            };

            Save(original_products);
        };

        Because of = () =>
        {
            ((AzureTableRepositorySession)products.Session).AutoSaveAfterNumberOperations = 3;

            var p0 = products.Get(original_products[0].PartitionKey, original_products[0].RowKey);
            var p1 = products.Get(original_products[1].PartitionKey, original_products[1].RowKey);
            var p2 = products.Get(original_products[2].PartitionKey, original_products[2].RowKey);
            var p3 = products.Get(original_products[3].PartitionKey, original_products[3].RowKey);

            products.Remove(p0);
            products.Remove(p1);
            products.Remove(p2);

            products.Remove(p3);
        };

        It should_execute_first_operation =
            () => GetProductFor(original_products[0]).ShouldBeNull();

        It should_execute_second_operation =
            () => GetProductFor(original_products[1]).ShouldBeNull();

        It should_execute_third_operation =
            () => GetProductFor(original_products[2]).ShouldBeNull();

        It should_not_execute_fourth_operation_as_the_save_changes_was_not_called_for_it =
            () => GetProductFor(original_products[3]).ShouldMatch(original_products[3]);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_executing_different_operations_with_auto_save_after_number_operations_set_to_3 : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;
        static AzureRepositoryTestProduct original_product_for_adding;

        Establish context = () =>
        {
            original_products = new []
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                }
            };

            Save(original_products);

            original_product_for_adding = new AzureRepositoryTestProduct
            {
                PartitionKey = "Green",
                RowKey = Guid.NewGuid().ToString(),
                Name = "A Green Product"
            };
        };

        Because of = () =>
        {
            ((AzureTableRepositorySession)products.Session).AutoSaveAfterNumberOperations = 3;

            var p0 = products.Get(original_products[0].PartitionKey, original_products[0].RowKey);
            var p1 = products.Get(original_products[1].PartitionKey, original_products[1].RowKey);
            var p2 = products.Get(original_products[2].PartitionKey, original_products[2].RowKey);

            p0.Name = "Changed";

            products.Add(original_product_for_adding);
            products.Update(p0);
            products.Remove(p1);

            products.Update(p2);
        };

        It should_execute_first_operation =
            () => GetProductFor(original_product_for_adding).ShouldMatch(original_product_for_adding);

        It should_execute_second_operation =
            () => GetProductFor(original_products[0]).ShouldMatch(original_products[0], "Changed");

        It should_execute_third_operation =
            () => GetProductFor(original_products[1]).ShouldBeNull();

        It should_not_execute_fourth_operation_as_the_save_changes_was_not_called_for_it =
            () => GetProductFor(original_products[2]).ShouldMatch(original_products[2]);
    }

    [Subject(typeof(AzureTableRepository<>))]
    class when_executing_different_operations_with_auto_save_after_number_operations_set_to_0_and_without_explicit_saving_changes : repository_test_context
    {
        static AzureRepositoryTestProduct[] original_products;
        static AzureRepositoryTestProduct original_product_for_adding;

        Establish context = () =>
        {
            original_products = new[]
            {
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                },
                new AzureRepositoryTestProduct
                {
                    PartitionKey = "Green",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "A Green Product"
                }
            };

            Save(original_products);

            original_product_for_adding = new AzureRepositoryTestProduct
            {
                PartitionKey = "Green",
                RowKey = Guid.NewGuid().ToString(),
                Name = "A Green Product"
            };
        };

        Because of = () =>
        {
            ((AzureTableRepositorySession)products.Session).AutoSaveAfterNumberOperations = 0;

            var p0 = products.Get(original_products[0].PartitionKey, original_products[0].RowKey);
            var p1 = products.Get(original_products[1].PartitionKey, original_products[1].RowKey);

            p0.Name = "Changed";

            products.Add(original_product_for_adding);
            products.Update(p0);
            products.Remove(p1);
        };

        It should_not_execute_the_add_operation =
            () => GetProductFor(original_product_for_adding).ShouldBeNull();

        It should_not_execute_the_update_operation =
            () => GetProductFor(original_products[0]).ShouldMatch(original_products[0]);

        It should_not_execute_the_remove_operation =
            () => GetProductFor(original_products[1]).ShouldMatch(original_products[1]);
    }

    class repository_test_context
    {
        protected static AzureTableRepository<AzureRepositoryTestProduct> products;

        Cleanup after_each =
            () => DropTable();

        Establish before_each = () =>
        {
            CreateTable();
            products = new AzureTableRepository<AzureRepositoryTestProduct>(CloudStorageAccount.DevelopmentStorageAccount.CreateCloudTableClient());
        };

        public static AzureRepositoryTestProduct GetProductFor(AzureRepositoryTestProduct matching, string tableName = null)
        {
            return GetProduct(matching.PartitionKey, matching.RowKey, tableName);
        }

        public static AzureRepositoryTestProduct GetProduct(string partitionKey, string rowKey, string tableName = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                tableName = EntityToNameMap.Get<AzureRepositoryTestProduct>();

            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);

            return (AzureRepositoryTestProduct)table.Execute(TableOperation.Retrieve<AzureRepositoryTestProduct>(partitionKey, rowKey)).Result;
        }

        protected static void Save(params AzureRepositoryTestProduct[] originalProducts)
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(EntityToNameMap.Get<AzureRepositoryTestProduct>());

            var operation = new TableBatchOperation();
            foreach (var product in originalProducts)
            {
                operation.Insert(product);
            }

            table.ExecuteBatch(operation);
        }

        public static void CreateTable(string tableName = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                tableName = EntityToNameMap.Get<AzureRepositoryTestProduct>();

            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
        }

        public static void DropTable(string tableName = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                tableName = EntityToNameMap.Get<AzureRepositoryTestProduct>();

            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            table.DeleteIfExists();
        }
    }
}
