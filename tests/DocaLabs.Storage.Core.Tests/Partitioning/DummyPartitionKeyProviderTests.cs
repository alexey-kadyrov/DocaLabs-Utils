using System;
using DocaLabs.Storage.Core.Partitioning;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Tests.Partitioning
{
    [Subject(typeof(DummyPartitionKeyProvider))]
    class when_dummy_partition_key_is_used
    {
        static DummyPartitionKeyProvider key_provider;

        Because of =
            () => key_provider = new DummyPartitionKeyProvider();

        It should_always_return_empty_guid =
            () => key_provider.GetPartitionKey().ShouldEqual(Guid.Empty);
    }
}
