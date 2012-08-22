using System;
using System.Collections.Generic;
using System.Linq;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Utils.Tests
{
    [Subject(typeof (ExceptionExtensions)), UnitTestTag]
    class when_tyring_to_find_existing_inner_exception_type_in_non_aggregate_exception
    {
        static Exception root_exception;
        static ICollection<InvalidOperationException> found;

        Establish context = () =>
        {
            var second = new InvalidOperationException("second");
            var first = new InvalidOperationException("first", second);
            root_exception = new Exception("root", first);
        };

        Because of =
            () => found = root_exception.Find<InvalidOperationException>();

        It should_find_all_unique_instances =
            () => found.Select(x => x.Message).ShouldContainOnly("first", "second");
    }

    [Subject(typeof(ExceptionExtensions)), UnitTestTag]
    class when_tyring_to_find_exception_in_non_aggregate_exception
    {
        static Exception root_exception;
        static ICollection<Exception> found;

        Establish context = () =>
        {
            var second = new Exception("second");
            var first = new Exception("first", second);
            root_exception = new Exception("root", first);
        };

        Because of =
            () => found = root_exception.Find<Exception>();

        It should_find_all_unique_instances =
            () => found.Select(x => x.Message).ShouldContainOnly("root", "first", "second");
    }
}
