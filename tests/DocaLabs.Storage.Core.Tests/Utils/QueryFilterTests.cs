using System;
using System.Collections.Generic;
using System.Linq;
using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using Machine.Specifications.Annotations;

namespace DocaLabs.Storage.Core.Tests.Utils
{
    class FilterOutHello : QueryFilter<string>
    {
        protected override IQueryable<string> Applying(IQueryable<string> target)
        {
            return from x in target where x != "Hello" select x;
        }
    }

    class FilterOutWorld : QueryFilter<string>
    {
        protected override IQueryable<string> Applying(IQueryable<string> target)
        {
            return from x in target where x != "World" select x;
        }
    }

    class QueryFilterTestsContext
    {
        protected static IQueryable<string> sample_collection;

        [UsedImplicitly] Establish before_each = () => sample_collection = new List<string>
        {
            "Hello",
            "World",
            "!"
        }.AsQueryable();
    }

    [Subject(typeof(QueryFilter<>)), UnitTestTag]
    class when_single_filter_is_applied_to_target_collection : QueryFilterTestsContext
    {
        It should_return_all_items_filtered_by_that_filter =
            () => new FilterOutHello().Apply(sample_collection).ShouldContainOnly("World", "!");
    }

    [Subject(typeof(QueryFilter<>)), UnitTestTag]
    class when_chain_of_filters_is_applied_to_target_collection : QueryFilterTestsContext
    {
        It should_return_all_items_filtered_by_that_chain =
            () => new FilterOutHello().Chain(new FilterOutWorld()).Apply(sample_collection).ShouldContainOnly("!");
    }

    [Subject(typeof(QueryFilter<>)), UnitTestTag]
    class when_chaining_null_filter
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new FilterOutWorld().Chain(new FilterOutHello(), null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_filters_argument =
            () => ((ArgumentNullException) actual_exception).ParamName.ShouldEqual("filters");
    }

    [Subject(typeof(QueryFilter<>)), UnitTestTag]
    class when_apply_is_called_for_null_target_queryable
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new FilterOutWorld().Chain(new FilterOutHello()).Apply(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_target_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("target");
    }

    [Subject(typeof(QueryFilterExtensions)), UnitTestTag]
    class when_chain_of_filters_is_applied_to_target_collection_using_apply_extension_method : QueryFilterTestsContext
    {
        It should_return_all_items_filtered_by_that_chain =
            () => sample_collection.Apply(new FilterOutHello().Chain(new FilterOutWorld())).ShouldContainOnly("!");
    }

    [Subject(typeof(QueryFilterExtensions)), UnitTestTag]
    class when_apply_extension_method_from_query_filter_extensions_is_called_with_null_collection
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => ((IQueryable<string>)null).Apply(new FilterOutWorld()));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_target_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("target");
    }

    [Subject(typeof(QueryFilterExtensions)), UnitTestTag]
    class when_apply_extension_method_from_query_filter_extensions_is_called_with_null_filter : QueryFilterTestsContext
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => sample_collection.Apply(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_filter_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("filter");
    }
}
