using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;

namespace DocaLabs.Storage.Core.Tests.Partitioning
{
    // ReSharper disable InconsistentNaming

    [Subject(typeof(PartitionException)), UnitTestTag]
    class when_partition_exception_is_newed_using_default_constructor : ExceptionIsNewedUsingDefaultConstructorContext<PartitionException>
    {
        Behaves_like<ExceptionIsNewedUsingDefaultConstructorBehaviour> a_standard_exception;
    }

    [Subject(typeof(PartitionException)), UnitTestTag]
    class when_partition_exception_is_newed_using_overload_constructor_with_message : ExceptionIsNewedUsingOverloadConstructorWithMessageContext<PartitionException>
    {
        Behaves_like<ExceptionIsNewedUsingOverloadConstructorWithMessageBehaviour> a_standard_exception;
    }

    [Subject(typeof(PartitionException)), UnitTestTag]
    class when_partition_exception_is_newed_using_overload_constructor_with_message_and_inner_exception : ExceptionIsNewedUsingOverloadConstructorWithMessageAndInnerExceptionContext<PartitionException>
    {
        Behaves_like<ExceptionIsNewedUsingOverloadConstructorWithMessageAndInnerExceptionBehaviour> a_standard_exception;
    }

    [Subject(typeof(PartitionException)), UnitTestTag]
    class when_partition_exception_is_serialized : ExceptionIsSerializedContext<PartitionException>
    {
        Behaves_like<ExceptionIsSerializedBehaviour> a_standard_exception;
    }

    // ReSharper restore InconsistentNaming
}
