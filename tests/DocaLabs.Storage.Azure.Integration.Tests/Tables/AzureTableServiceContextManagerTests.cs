using System;
using DocaLabs.Storage.Azure.Tables;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using Machine.Specifications.Annotations;

namespace DocaLabs.Storage.Azure.Integration.Tests.Tables
{
    class AzureTableServiceContextManagerTestsContext
    {
        [UsedImplicitly] Cleanup after_each = () =>
        {
            AzureStorageFactory.UseDevelopmentStorageAccount = false;
        };

        [UsedImplicitly] Establish before_each = () =>
        {
            AzureStorageFactory.UseDevelopmentStorageAccount = true;
        };
    }

    [Subject(typeof(AzureTableServiceContextManager)), IntegrationTag]
    class when_data_service_context_manager_is_newed_with_null_base_address_argument : AzureTableServiceContextManagerTestsContext
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new AzureTableServiceContextManager(null, AzureStorageFactory.CreateAccount().Credentials));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_base_address_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("baseAddress");
    }

    [Subject(typeof(AzureTableServiceContextManager)), IntegrationTag]
    class when_data_service_context_manager_is_newed_with_white_space_base_address_argument : AzureTableServiceContextManagerTestsContext
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new AzureTableServiceContextManager("  ", AzureStorageFactory.CreateAccount().Credentials));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_base_address_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("baseAddress");
    }

    [Subject(typeof(AzureTableServiceContextManager)), IntegrationTag]
    class when_data_service_context_manager_is_newed_with_null_credentials_argument : AzureTableServiceContextManagerTestsContext
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new AzureTableServiceContextManager(AzureStorageFactory.CreateAccount().TableEndpoint.ToString(), null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_credentials_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("credentials");
    }
}
