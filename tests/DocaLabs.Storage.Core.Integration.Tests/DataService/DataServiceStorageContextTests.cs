using System;
using System.Data.Services.Client;
using DocaLabs.Storage.Core.DataService;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;

namespace DocaLabs.Storage.Core.Integration.Tests.DataService
{
    [Subject(typeof(DataServiceStorageContext)), IntegrationTag]
    class when_data_service_context_wrapper_is_newed_with_null_service_root_argument
    {
        static Exception actual_exception;

        Because of = () =>
        {
            actual_exception = Catch.Exception(() => new DataServiceStorageContext(null));
        };

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_context_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("context");
    }

    [Subject(typeof(DataServiceStorageContext)), IntegrationTag]
    class when_batch_operation_requests_save_options
    {
        class DataServiceContextStorageUnderTest : DataServiceStorageContext
        {
            public DataServiceContextStorageUnderTest(DataServiceContext context)
                : base(context)
            {
            }

            public static SaveChangesOptions CallAddSafelyBatchOption(SaveChangesOptions options)
            {
                return AddSafelyBatchOption(options);
            }
        }

        static SaveChangesOptions actual_options;

        Because of =
            () => actual_options = DataServiceContextStorageUnderTest.CallAddSafelyBatchOption(SaveChangesOptions.ContinueOnError | SaveChangesOptions.ReplaceOnUpdate);

        It should_remove_continue_on_error_and_add_batch =
            () => actual_options.ShouldEqual(SaveChangesOptions.ReplaceOnUpdate | SaveChangesOptions.Batch);
    }
}
