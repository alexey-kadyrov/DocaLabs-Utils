﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DocaLabs.Http.Client.Mapping;
using DocaLabs.Http.Client.Mapping.Attributes;
using Machine.Specifications;

namespace DocaLabs.Http.Client.Tests.Mapping
{
    [Subject(typeof(SeparatedCollectionConverter))]
    class when_separated_collection_converter_is_newed
    {
        static SeparatedCollectionConverter converter;

        Because of = 
            () => converter = new SeparatedCollectionConverter(typeof(TestClass).GetProperty("Countries"));

        It should_return_pipe_as_item_separator =
            () => converter.Separator.ShouldEqual('|');

        class TestClass
        {
            // ReSharper disable UnusedMember.Local
            [SeparatedCollectionConverter(Separator = ';')]
            public IEnumerable<string> Countries { get; set; }
            // ReSharper restore UnusedMember.Local
        }
    }

    [Subject(typeof(SeparatedCollectionConverter))]
    class when_separated_collection_converter_is_newed_with_null_property_info
    {
        static Exception exception;

        Because of =
            () => exception = Catch.Exception(() => new SeparatedCollectionConverter(null));

        It should_throw_argument_null_exception =
            () => exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_info_argument =
            () => ((ArgumentNullException) exception).ParamName.ShouldEqual("info");
    }

    [Subject(typeof(SeparatedCollectionConverter))]
    class when_separated_collection_converter_is_used
    {
        static PropertyInfo property_info;
        static TestClass instance;
        static IConvertProperty converter;
        static IEnumerable<KeyValuePair<string, IList<string>>> result;

        Establish context = () =>
        {
            property_info = typeof(TestClass).GetProperty("Values");
            instance = new TestClass
            {
                Values = new[] { 27, 42 }
            };

            converter = new SeparatedCollectionConverter(property_info)
            {
                Separator = ','
            };
        };

        Because of =
            () => result = converter.GetValue(instance);

        It should_be_able_to_get_the_key_as_property_name =
            () => result.First().Key.ShouldEqual("Values");

        It should_be_able_to_get_value_of_property =
            () => result.First().Value[0].ShouldEqual("27,42");

        class TestClass
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            [SeparatedCollectionConverter(Separator = ';')]
            public IEnumerable<int> Values { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }
    }

    [Subject(typeof(SeparatedCollectionConverter))]
    class when_separated_collection_converter_is_used_with_together_with_query_parameter_where_name_and_format_are_not_set
    {
        static PropertyInfo property_info;
        static TestClass instance;
        static IConvertProperty converter;
        static IEnumerable<KeyValuePair<string, IList<string>>> result;

        Establish context = () =>
        {
            property_info = typeof(TestClass).GetProperty("Values");
            instance = new TestClass
            {
                Values = new[] { 27, 42 }
            };

            converter = new SeparatedCollectionConverter(property_info)
            {
                Separator = ','
            };
        };

        Because of =
            () => result = converter.GetValue(instance);

        It should_be_able_to_get_the_key_as_property_name =
            () => result.First().Key.ShouldEqual("Values");

        It should_be_able_to_get_value_of_property =
            () => result.First().Value[0].ShouldEqual("27,42");

        class TestClass
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            [SeparatedCollectionConverter(Separator = ';'), QueryParameter]
            public IEnumerable<int> Values { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }
    }

    [Subject(typeof(SeparatedCollectionConverter))]
    class when_separated_collection_converter_is_used_on_property_which_name_is_redefined_using_query_parameter_attribute
    {
        static PropertyInfo property_info;
        static TestClass instance;
        static IConvertProperty converter;
        static IEnumerable<KeyValuePair<string, IList<string>>> result;

        Establish context = () =>
        {
            property_info = typeof(TestClass).GetProperty("Values");
            instance = new TestClass
            {
                Values = new[] { 27, 42 }
            };

            converter = new SeparatedCollectionConverter(property_info)
            {
                Separator = ','
            };
        };

        Because of =
            () => result = converter.GetValue(instance);

        It should_be_able_to_get_the_key_as_the_redefined_name =
            () => result.First().Key.ShouldEqual("Hello World");

        It should_be_able_to_get_value_of_property =
            () => result.First().Value[0].ShouldEqual("27,42");

        class TestClass
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            [SeparatedCollectionConverter(Separator = ';'), QueryParameter(Name = "Hello World")]
            public IEnumerable<int> Values { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }
    }

    [Subject(typeof(SeparatedCollectionConverter))]
    class when_separated_collection_converter_is_used_on_property_with_custom_format_applied
    {
        static PropertyInfo property_info;
        static TestClass instance;
        static IConvertProperty converter;
        static IEnumerable<KeyValuePair<string, IList<string>>> result;

        Establish context = () =>
        {
            property_info = typeof(TestClass).GetProperty("Values");
            instance = new TestClass
            {
                Values = new[] { 27, 42 }
            };

            converter = new SeparatedCollectionConverter(property_info)
            {
                Separator = ','
            };
        };

        Because of =
            () => result = converter.GetValue(instance);

        It should_be_able_to_get_the_key_as_the_redefined_name =
            () => result.First().Key.ShouldEqual("Values");

        It should_be_able_to_get_value_of_property =
            () => result.First().Value[0].ShouldEqual("1B,2A");

        class TestClass
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            [SeparatedCollectionConverter(Separator = ';'), QueryParameter(Format = "X")]
            public IEnumerable<int> Values { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }
    }
}
