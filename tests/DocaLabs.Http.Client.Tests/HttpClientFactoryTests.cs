﻿using System;
using System.Reflection;
using DocaLabs.Http.Client.Tests._Utils;
using Machine.Specifications;

namespace DocaLabs.Http.Client.Tests
{
    [Subject(typeof(HttpClientFactory))]
    class when_creating_instance_for_service_without_specifying_base_class
    {
        static IServiceWithQueryAndResult instance;

        Because of =
            () => instance = HttpClientFactory.CreateInstance<IServiceWithQueryAndResult>(new Uri("http://foo.bar/"));

        // it's impossible to verify the call as it will try to execute the "real" pipeline
        It should_be_able_to_create_the_instance =
            () => instance.ShouldNotBeNull();
    }

    [Subject(typeof(HttpClientFactory))]
    class when_creating_instance_for_service_with_query_and_result_for_generic_type_difinition_as_base_class
    {
        static IServiceWithQueryAndResult2 instance;

        Because of =
            () => instance = HttpClientFactory.CreateInstance<IServiceWithQueryAndResult2>(typeof(TestHttpClient<,>), new Uri("http://foo.bar/"));

        It should_be_able_to_call_the_service =
            () => instance.GetResult(new TestsQuery { Value = "Hello!" }).Value.ShouldEqual("Hello!");
    }

    [Subject(typeof(HttpClientFactory))]
    class when_creating_instance_for_service_for_fully_defined_generic_type
    {
        static IServiceWithQueryAndResult3 instance;

        Because of =
            () => instance = HttpClientFactory.CreateInstance<IServiceWithQueryAndResult3>(typeof(TestHttpClient<TestsQuery, TestsQuery>), new Uri("http://foo.bar/"));

        It should_be_able_to_call_the_service =
            () => instance.GetResult(new TestsQuery { Value = "Hello!" }).Value.ShouldEqual("Hello!");
    }

    [Subject(typeof(HttpClientFactory))]
    class when_creating_instance_for_service_for_non_generic_base_type
    {
        static IServiceWithQueryAndResult5 instance;

        Because of =
            () => instance = HttpClientFactory.CreateInstance<IServiceWithQueryAndResult5>(typeof(TestHttpClient2), new Uri("http://foo.bar/"));

        It should_be_able_to_call_the_service =
            () => instance.GetResult(new TestsQuery { Value = "Hello!" }).Value.ShouldEqual("Hello!");
    }

    [Subject(typeof(HttpClientFactory))]
    class when_creating_instance_several_times_for_the_same_interface
    {
        static IServiceWithQueryAndResult4 instance;

        Establish context =
            () => HttpClientFactory.CreateInstance<IServiceWithQueryAndResult4>(typeof(TestHttpClient<,>), new Uri("http://foo.bar/"));

        Because of =
            () => instance = HttpClientFactory.CreateInstance<IServiceWithQueryAndResult4>(typeof(TestHttpClient<,>), new Uri("http://foo.bar/"));

        It should_still_be_able_to_create_instane_and_call_the_service =
            () => instance.GetResult(new TestsQuery { Value = "Hello!" }).Value.ShouldEqual("Hello!");
    }

    [Subject(typeof(HttpClientFactory))]
    class when_creating_instance_several_times_for_the_same_interface_but_different_base_classes
    {
        static IServiceWithQueryAndResult6 instance;

        Establish context =
            () => HttpClientFactory.CreateInstance<IServiceWithQueryAndResult6>(typeof(TestHttpClient2), new Uri("http://foo.bar/"));

        Because of =
            () => instance = HttpClientFactory.CreateInstance<IServiceWithQueryAndResult6>(typeof(TestHttpClient<,>), new Uri("http://foo.bar/"));

        It should_create_instance_with_the_most_first_base_class =
            () => instance.GetType().BaseType.ShouldEqual(typeof(TestHttpClient2));

        It should_still_be_able_to_create_instane_and_call_the_service =
            () => instance.GetResult(new TestsQuery { Value = "Hello!" }).Value.ShouldEqual("Hello!");
    }

    [Subject(typeof(HttpClientFactory))]
    class when_creating_instance_for_service_decorated_with_attributes
    {
        static IDecoratedService instance;

        Because of =
            () => instance = HttpClientFactory.CreateInstance<IDecoratedService>(typeof(TestHttpClient<,>), new Uri("http://foo.bar/"));

        It should_be_able_to_call_the_service =
            () => instance.GetResult(new TestsQuery { Value = "Hello!" }).Value.ShouldEqual("Hello!");

        It should_not_transfer_attribute_that_is_not_defined_for_class =
            () => instance.GetType().GetCustomAttribute<InterfaceOnlyAttribute>().ShouldBeNull();

        It should_transfer_attribute_that_is_defined_for_class_and_has_properties_fields_and_constructor =
            () => instance.GetType().GetCustomAttribute<ClassAttributeWithFieldsPropertiesAndConstructorArgsAttribute>()
                          .ShouldMatch(x => x.FromConstructorArg == "one" && x.Field == "two" && x.Property == "three");
    }

    [Subject(typeof(HttpClientFactory))]
    class when_creating_instance_for_service_with_result_only
    {
        static IServiceWithResultOnly instance;

        Because of =
            () => instance = HttpClientFactory.CreateInstance<IServiceWithResultOnly>(typeof(TestHttpClient<,>), new Uri("http://foo.bar/"));

        It should_be_able_to_call_the_service =
            () => instance.Get().ShouldBeOfType<TestResult>();
    }

    [Subject(typeof(HttpClientFactory))]
    class when_creating_instance_for_service_with_query_only
    {
        static IServiceWithQueryOnly instance;

        Because of =
            () => instance = HttpClientFactory.CreateInstance<IServiceWithQueryOnly>(typeof(TestHttpClient<,>), new Uri("http://foo.bar/"));

        It should_be_able_to_call_the_service = () =>
        {
            instance.Post(new TestsQuery());
            instance.GetType().GetProperty("ExecutionMarker").GetValue(instance, null).ShouldEqual("Pipeline was executed.");
        };
    }

    [Subject(typeof(HttpClientFactory))]
    class when_creating_instance_for_service_without_query_or_result
    {
        static IServiceWithoutQueryOrResult instance;

        Because of =
            () => instance = HttpClientFactory.CreateInstance<IServiceWithoutQueryOrResult>(typeof(TestHttpClient<,>), new Uri("http://foo.bar/"));

        It should_be_able_to_call_the_service = () =>
        {
            instance.Do();
            instance.GetType().GetProperty("ExecutionMarker").GetValue(instance, null).ShouldEqual("Pipeline was executed.");
        };
    }
}
