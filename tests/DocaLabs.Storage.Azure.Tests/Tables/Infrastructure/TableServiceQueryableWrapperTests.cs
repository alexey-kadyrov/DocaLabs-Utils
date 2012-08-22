using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DocaLabs.Storage.Azure.Tables;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Azure.Tests.Tables.Infrastructure
{
    [Subject(typeof (TableServiceQueryableWrapper<string>)), UnitTestTag]
    class when_table_service_queryable_wrapper_is_newed_with_null_query_argument
    {
        static Exception actual_exception;

        Because of =
            () => actual_exception = Catch.Exception(() => new TableServiceQueryableWrapper<string>(null));

        It shoudl_throw_argument_null_eception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_query_argument =
            () => ((ArgumentNullException) actual_exception).ParamName.ShouldEqual("query");
    }

    [Subject(typeof(TableServiceQueryableWrapper<string>)), UnitTestTag]
    class when_table_service_queryable_wrapper_is_newed
    {
        static IQueryable<string> queryable_base;
        static TableServiceQueryableWrapper<string> wrapper;

        Establish context = 
            () => queryable_base = new List<string> { "1", "2", "3" }.AsQueryable();

        Because of =
            () => wrapper = new TableServiceQueryableWrapper<string>(queryable_base);

        It should_be_able_to_enumerate_through_all_wrapped_items =
            () => wrapper.ShouldContainOnly("1", "2", "3");

        It should_be_able_to_enumerate_through_all_wrapped_items_using_implicit_enumartor =
            () => ((IEnumerable)wrapper).GetEnumerator().ShouldContainOnlySimilar((x, y) => x == y, "1", "2", "3");

        It should_be_able_to_chain_where_extensions_and_iterate_through_result =
            () => wrapper.Where(x => x != "1").Where(x => x != "2").ShouldContainOnly("3");

        It should_be_able_to_project =
            () => wrapper.Select(x => x + x).ShouldContainOnly("11", "22", "33");

        It should_be_able_to_get_single_item =
            () => wrapper.First(x => x == "2").ShouldEqual("2");

        It should_be_able_to_get_firts_item =
            () => wrapper.First().ShouldEqual("1");

        It should_count_items_correctly =
            () => wrapper.Count().ShouldEqual(3);

        It should_count_items_correctly_using_non_generic_implementation =
            () => NonGenericCount(wrapper).ShouldEqual(3);

        It should_be_able_to_get_single_item_using_non_generic_implementation =
            () => NonGenericWhereEqual2(wrapper, x => x == "2").ShouldContainOnlySimilar<string, string>((x, y) => x == y, "2");

        It element_type_should_be_string_type =
            () => wrapper.ElementType.ShouldEqual(typeof (string));

        static int NonGenericCount(IQueryable source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return (int)source.Provider.Execute(
                Expression.Call(typeof(Queryable), "Count", new [] { source.ElementType }, new [] { source.Expression }));
        }

        static IQueryable NonGenericWhereEqual2(IQueryable source, Expression<Func<string, bool>> expression)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Where", new [] { source.ElementType }, new []
            {
                source.Expression,
                Expression.Quote(expression)
            }));
        }
    }
}
