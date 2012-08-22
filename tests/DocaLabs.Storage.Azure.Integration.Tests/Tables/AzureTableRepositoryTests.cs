using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using DocaLabs.Storage.Azure.Integration.Tests.TestProviders;
using DocaLabs.Storage.Azure.Tables;
using DocaLabs.Storage.Core.DataService;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using Machine.Specifications.Annotations;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Azure.Integration.Tests.Tables
{
    // ReSharper disable RemoveToList.2 (As the TableServiceContext doesn't support the Count directly)
    // ReSharper disable ReplaceWithSingleCallToFirst (as the Linq for TableServiceContext doesn't support it)

    class AzureTableRepositoryTestsContext
    {
        protected const int BatchSize = 100;

        protected static Random random;

        protected static Product first_product;
        protected static Product second_product;
        protected static Product third_product;

        [UsedImplicitly] Cleanup after_each = () =>
        {
            AzureStorageFactory.CreateCloudTableClient().DeleteTableIfExist(Product.TableName);

            AzureStorageFactory.UseDevelopmentStorageAccount = false;
        };

        [UsedImplicitly] Establish before_each = () =>
        {
            random = new Random();

            AzureStorageFactory.UseDevelopmentStorageAccount = true;

            var client = AzureStorageFactory.CreateCloudTableClient();
            client.DeleteTableIfExist(Product.TableName);
            client.CreateTableIfNotExist(Product.TableName);
        };

        protected static Product[] CreateAndSaveDefaultProducts()
        {
            var products = new Product[3];

            products[0] = first_product = new Product("category-1", Guid.NewGuid())
            {
                Name = "First Product",
                Price = 99.95,
                ReleaseDate = new DateTime(2011, 12, 29),
                Rating = 3
            };

            products[1] = second_product = new Product("category-1", Guid.NewGuid())
            {
                Name = "Second Product",
                Price = 75.05,
                ReleaseDate = new DateTime(2011, 12, 30),
                Rating = 5
            };

            products[2] = third_product = new Product("category-2", Guid.NewGuid())
            {
                Name = "Third Product",
                Price = 33.15,
                ReleaseDate = new DateTime(2011, 12, 31),
                Rating = 1
            };

            var context = AzureStorageFactory.GetTableServiceContext();

            context.AddObject(Product.TableName, first_product);
            context.AddObject(Product.TableName, second_product);
            context.AddObject(Product.TableName, third_product);

            context.SaveChangesWithRetries();

            return products;
        }
   
        protected static Product[] CreateProducts(string category, int count)
        {
            var products = new Product[count];

            while (count > 0)
            {
                --count;

                products[count] = NewProduct(category);
            }

            return products;
        }

        protected static Product[] CreateAndSaveProducts(string category, int count)
        {
            var context = AzureStorageFactory.GetTableServiceContext();

            var products = new Product[count];

            while (count > 0)
            {
                --count;

                products[count] = NewProduct(category);

                context.AddObject(Product.TableName, products[count]);

                if (context.Entities.Count != BatchSize)
                    continue;

                context.SaveChangesWithRetries(SaveChangesOptions.Batch);

                context = AzureStorageFactory.GetTableServiceContext();
            }

            if (context.Entities.Count > 0)
                context.SaveChangesWithRetries(SaveChangesOptions.Batch);

            return products;
        }
   
        protected static Product NewProduct(string category)
        {
            var productId = Guid.NewGuid();

            return new Product(category, productId)
            {
                Name = "Product " + productId,
                Price = Math.Round(random.NextDouble() * random.Next(1, 100), 2),
                ReleaseDate = new DateTime(2011, 12, random.Next(1, 31)),
                Rating = random.Next(1, 5)
            };
        }

        protected static IQueryable<Product> GetAllPersistedProducts()
        {
            return new CloudTableQuery<Product>(AzureStorageFactory.GetTableServiceContext().CreateQuery<Product>(Product.TableName));
        }
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_looking_for_existing_entity_by_key : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;
        static Product found_product;

        Establish context = () =>
        {
            CreateAndSaveDefaultProducts();
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());
        };

        Because of =
            () => found_product = repository.Find(second_product.PartitionKey, second_product.RowKey);

        It should_find_matching_entity =
            () => found_product.ShouldBeSimilar(second_product, Product.Compare);
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_looking_for_non_existing_entity_by_key : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;
        static Product found_product;

        Establish context = () =>
        {
            CreateAndSaveDefaultProducts();
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());
        };

        Because of =
            () => found_product = repository.Find(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        It should_return_null =
            () => found_product.ShouldBeNull();
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_querying_repository : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;
        static Product[] category_3_products;
        static Product[] category_4_products;

        Establish context = () =>
        {
            CreateAndSaveDefaultProducts();
            category_3_products = CreateAndSaveProducts("category-3", 1200);
            category_4_products = CreateAndSaveProducts("category-4", 999);
        };

        Because of =
            () => repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());

        It should_count_all_entities =
            () => repository.ToList().Count.ShouldEqual(2202);

        It should_get_first_product_entity_by_its_name =
            () => repository.Where(x => x.Name == first_product.Name).ShouldContainOnlySimilar(Product.Compare, first_product);

        It should_get_second_product_entity_by_its_name_and_id_by_chaining_where_operators =
            () => repository.Where(x => x.Name == second_product.Name).Where(x => x.RowKey == second_product.RowKey).ShouldContainOnlySimilar(Product.Compare, second_product);

        It should_get_third_product_entity_by_its_name_and_rating =
            () => repository.Where(x => x.Name == third_product.Name && x.Rating == third_product.Rating).ShouldContainOnlySimilar(Product.Compare, third_product);

        It should_get_all_1200_entities_of_category_3_which_span_two_partitions =
            () => repository.Where(x => x.PartitionKey == "category-3").ShouldContainOnlySimilar(Product.Compare, category_3_products);

        It should_get_all_999_entities_of_category_4_which_span_one_partition =
            () => repository.Where(x => x.PartitionKey == "category-4").ShouldContainOnlySimilar(Product.Compare, category_4_products);
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_enumerating_repository_using_explicit_ienumerable_interface : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;
        static List<Product> all_products;

        Establish context = () =>
        {
            all_products = new List<Product>();
            all_products.AddRange(CreateAndSaveDefaultProducts());
            all_products.AddRange(CreateAndSaveProducts("category-3", 1200));
            all_products.AddRange(CreateAndSaveProducts("category-4", 999));
        };

        Because of =
            () => repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());

        It should_contain_all_entities =
            () => ((IEnumerable)repository).GetEnumerator().ShouldContainOnlySimilar((x, y) => Product.Compare((Product)x, (Product)y), all_products.Cast<object>().ToArray());
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_adding_single_entity : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;
        static Product[] new_products;
        static List<Product> all_products;

        Establish context = () =>
        {
            all_products = new List<Product>();
            all_products.AddRange(CreateAndSaveDefaultProducts());
            all_products.AddRange(new_products = CreateProducts("category-3", 1));
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());
        };

        Because of = () =>
        {
            repository.Add(new_products[0]);
            repository.Unit.SaveChanges();
        };

        It should_be_persisted =
            () => GetAllPersistedProducts().ShouldContainOnlySimilar(Product.Compare, all_products.ToArray());
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_adding_several_entities : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;
        static Product[] new_products;
        static List<Product> all_products;

        Establish context = () =>
        {
            all_products = new List<Product>();
            all_products.AddRange(CreateAndSaveDefaultProducts());
            all_products.AddRange(new_products = CreateProducts("category-3", 3));
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());
        };

        Because of = () =>
        {
            repository.Add(new_products[0]);
            repository.Add(new_products[1]);
            repository.Add(new_products[2]);
            repository.Unit.SaveChanges();
        };

        It should_be_persisted =
            () => GetAllPersistedProducts().ShouldContainOnlySimilar(Product.Compare, all_products.ToArray());
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_adding_several_entities_and_saving_then_in_a_batch : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;
        static Product[] new_products;
        static List<Product> all_products;

        Establish context = () =>
        {
            all_products = new List<Product>();
            all_products.AddRange(CreateAndSaveDefaultProducts());
            all_products.AddRange(new_products = CreateProducts("category-3", 3));
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());
        };

        Because of = () =>
        {
            repository.Add(new_products[0]);
            repository.Add(new_products[1]);
            repository.Add(new_products[2]);
            repository.DataService.SaveBatchChanges();
        };

        It should_be_persisted =
            () => GetAllPersistedProducts().ShouldContainOnlySimilar(Product.Compare, all_products.ToArray());
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_removing_single_entity : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;

        Establish context = () =>
        {
            CreateAndSaveDefaultProducts();
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());
        };

        Because of = () =>
        {
            var entityToBeRemoved = repository.Where(x => x.PartitionKey == second_product.PartitionKey && x.RowKey == second_product.RowKey).First();
            repository.Remove(entityToBeRemoved);
            repository.Unit.SaveChanges();
        };

        It should_be_removed =
            () => GetAllPersistedProducts().ShouldContainOnlySimilar(Product.Compare, first_product, third_product);
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_removing_several_entities : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;

        Establish context = () =>
        {
            CreateAndSaveDefaultProducts();
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());
        };

        Because of = () =>
        {
            var entityToBeRemoved2 = repository.Where(x => x.PartitionKey == second_product.PartitionKey && x.RowKey == second_product.RowKey).First();
            repository.Remove(entityToBeRemoved2);

            var entityToBeRemoved3 = repository.Where(x => x.PartitionKey == third_product.PartitionKey && x.RowKey == third_product.RowKey).First();
            repository.Remove(entityToBeRemoved3);

            repository.Unit.SaveChanges();
        };

        It should_be_removed =
            () => GetAllPersistedProducts().ShouldContainOnlySimilar(Product.Compare, first_product);
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_updating_single_entity : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;

        Establish context = () =>
        {
            CreateAndSaveDefaultProducts();
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());
        };

        Because of = () =>
        {
            var entity = repository.Find(second_product.PartitionKey, second_product.RowKey);
            
            entity.Name = "Updated$." + entity.Name;

            second_product.Name = entity.Name;

            repository.Update(entity);

            repository.Unit.SaveChanges();
        };

        It should_be_persisted =
            () => GetAllPersistedProducts().ShouldContainOnlySimilar(Product.Compare, first_product, second_product, third_product);
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_updating_several_entities : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;

        Establish context = () =>
        {
            CreateAndSaveDefaultProducts();
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());
        };

        Because of = () =>
        {
            var entity2 = repository.Find(second_product.PartitionKey, second_product.RowKey);
            entity2.Name = "Updated$." + entity2.Name;
            second_product.Name = entity2.Name;
            repository.Update(entity2);

            var entity3 = repository.Where(x => x.PartitionKey == third_product.PartitionKey && x.RowKey == third_product.RowKey).First();
            entity3.Name = "Updated$." + entity3.Name;
            third_product.Name = entity3.Name;
            repository.Update(entity3);

            repository.Unit.SaveChanges();
        };

        It should_be_persisted =
            () => GetAllPersistedProducts().ShouldContainOnlySimilar(Product.Compare, first_product, second_product, third_product);
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_save_changes_default_options_are_changed_using_cast_to_data_service_context_storgae_interface : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;

        Establish context = () =>
        {
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());
        };

        Because of = () =>
        {
            ((IDataServiceStorageContext)repository.Unit).SaveChangesDefaultOptions = SaveChangesOptions.Batch;
        };

        It should_be_possible_to_read_them_back =
            () => ((IDataServiceStorageContext)repository.Unit).SaveChangesDefaultOptions.ShouldEqual(SaveChangesOptions.Batch);
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_repository_is_newed : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;

        Because of = () =>
        {
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());
        };

        It service_context_should_be_of_table_service_context_type =
            () => ((IDataServiceStorageContext)repository.Unit).Context.ShouldBeOfType<TableServiceContext>();

        It table_name_should_be_docalabstestproducts =
            () => repository.TableName.ShouldEqual("DocaLabsTestProducts");

        It element_type_should_be_product_type =
            () => repository.ElementType.ShouldEqual(typeof(Product));
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_repository_is_newed_using_overload_constructor_with_table_name : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;

        Because of = () =>
        {
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager(), "DocaLabsTestExplicitTableName");
        };

        It should_use_that_table_name =
            () => repository.TableName.ShouldEqual("DocaLabsTestExplicitTableName");
    }

    [Subject(typeof(AzureTableRepository<>)), IntegrationTag]
    class when_performing_several_different_opeartions_on_repository : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;
        static Product[] new_products;
        static List<Product> all_products;

        Establish context = () =>
        {
            all_products = new List<Product>();
            all_products.AddRange(CreateAndSaveDefaultProducts());
            all_products.AddRange(new_products = CreateProducts("category-3", 3));
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager());
        };

        Because of = () =>
        {
            repository.AddRange(new_products);

            repository.Unit.SaveChanges();

            new_products[0].Name = "Updated$." + new_products[0].Name;

            repository.Update(new_products[0]);

            repository.RemoveRange(repository.Where(x => x.PartitionKey == first_product.PartitionKey && x.RowKey == first_product.RowKey));

            repository.Unit.SaveChanges();

            all_products.Remove(first_product);
        };

        It should_be_persisted =
            () => GetAllPersistedProducts().ShouldContainOnlySimilar(Product.Compare, all_products.ToArray());
    }

    [Subject(typeof(AzureTableServiceContextManager)), IntegrationTag]
    class when_azure_table_service_context_manager_is_newed_using_constractor_with_explicit_base_address_and_credentials : AzureTableRepositoryTestsContext
    {
        static IDataServiceRepository<Product> repository;
        static string base_address;
        static StorageCredentials credentials;

        Establish context = () =>
        {
            var account = AzureStorageFactory.CreateAccount();
            base_address = account.TableEndpoint.ToString();
            credentials = account.Credentials;
            CreateAndSaveDefaultProducts();
        };

        Because of = () =>
            repository = new AzureTableRepository<Product>(new AzureTableServiceContextManager(base_address, credentials));

        It repository_should_contain_all_entities_or_in_other_words_being_operational =
            () => repository.ShouldContainOnlySimilar(Product.Compare, first_product, second_product, third_product);
    }

    // ReSharper restore ReplaceWithSingleCallToFirst
    // ReSharper restore RemoveToList.2
}
