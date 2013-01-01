using DocaLabs.Storage.Core.Repositories;
using Machine.Specifications;
using Moq;
using System;
using System.Collections.Generic;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Tests.Repositories
{
    [Subject(typeof(RepositoryExtensions))]
    class when_adding_a_null_collection_of_entities
    {
        static Mock<IRepository<Product>> products;
        static Exception actual_exception;

        Establish context =
            () => products = new Mock<IRepository<Product>>();

        Because of =
            () => actual_exception = Catch.Exception(() => products.Object.AddRange((IEnumerable<Product>)null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_items_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("items");
    }

    [Subject(typeof(RepositoryExtensions))]
    class when_adding_to_a_null_repository_with_collection_of_entities
    {
        static Exception actual_exception;
        static List<Product> original_products;

        Establish context = () =>
        {
            original_products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "A Green Product 1"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "A Green Product 1"
                }
            };
        };

        Because of =
            () => actual_exception = Catch.Exception(() => ((IRepository<Product>)null).AddRange(original_products));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_repository_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("repository");
    }

    [Subject(typeof(RepositoryExtensions))]
    class when_adding_to_a_null_repository_with_variable_param_list_of_entities
    {
        static Exception actual_exception;
        static Product[] original_products;

        Establish context = () =>
        {
            original_products = new []
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "A Green Product 1"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "A Green Product 1"
                }
            };
        };

        Because of = 
            () => actual_exception = Catch.Exception(() => ((IRepository<Product>)null).AddRange(original_products[0], original_products[1]));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_repository_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("repository");
    }

    [Subject(typeof(RepositoryExtensions))]
    class when_removing_a_null_collection_of_entities
    {
        static Mock<IRepository<Product>> products;
        static Exception actual_exception;

        Establish context =
            () => products = new Mock<IRepository<Product>>();

        Because of =
            () => actual_exception = Catch.Exception(() => products.Object.RemoveRange((IEnumerable<Product>)null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_items_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("items");
    }

    [Subject(typeof(RepositoryExtensions))]
    class when_removing_from_a_null_repository_with_collection_of_entities
    {
        static Exception actual_exception;
        static List<Product> original_products;

        Establish context = () =>
        {
            original_products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "A Green Product 1"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "A Green Product 1"
                }
            };
        };

        Because of =
            () => actual_exception = Catch.Exception(() => ((IRepository<Product>)null).RemoveRange(original_products));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_repository_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("repository");
    }

    [Subject(typeof(RepositoryExtensions))]
    class when_removing_from_a_null_repository_with_variable_param_list_of_entities
    {
        static Exception actual_exception;
        static Product[] original_products;

        Establish context = () =>
        {
            original_products = new[]
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "A Green Product 1"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "A Green Product 1"
                }
            };
        };

        Because of =
            () => actual_exception = Catch.Exception(() => ((IRepository<Product>)null).RemoveRange(original_products[0], original_products[1]));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_repository_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("repository");
    }

    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
