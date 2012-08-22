using DocaLabs.Storage.Core.Utils;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Core.Tests.Utils
{
    class SampleEntity
    {
    }

    [Subject(typeof(EntityTableNameResolver)), UnitTestTag]
    class when_resolve_on_current_provider_is_called
    {
        static Mock<IEntityTableNameProvider> mock_provider;

        Cleanup after_each = () => EntityTableNameResolver.Provider = null;

        Establish context = () =>
        {
            mock_provider = new Mock<IEntityTableNameProvider>();
            mock_provider.Setup(x => x.Resolve<SampleEntity>()).Returns("resolve-was-called");
            mock_provider.Setup(x => x.Resolve(typeof(SampleEntity))).Returns("resolve-was-called");
        };

        Because of =
            () => EntityTableNameResolver.Provider = mock_provider.Object;

        It should_return_value_supplied_by_that_provider =
            () => EntityTableNameResolver.Resolve<SampleEntity>().ShouldEqual("resolve-was-called");
    }

    [Subject(typeof(EntityTableNameResolver)), UnitTestTag]
    class entity_table_name_resolver_in_default_state
    {
        It should_have_non_null_provider =
            () => EntityTableNameResolver.Provider.ShouldNotBeNull();

        It should_use_default_clock_provider =
            () => EntityTableNameResolver.Provider.ShouldBeOfType<EntityTableNameProvider>();
    }

    [Subject(typeof(EntityTableNameResolver)), UnitTestTag]
    class when_provider_is_set_to_null
    {
        Cleanup after_each =
            () => EntityTableNameResolver.Provider = null;

        Establish context =
            () => EntityTableNameResolver.Provider = new Mock<IEntityTableNameProvider>().Object;

        Because of =
            () => EntityTableNameResolver.Provider = null;

        It getter_should_return_non_null_provider =
            () => EntityTableNameResolver.Provider.ShouldNotBeNull();

        It should_use_default_clock_provider =
            () => EntityTableNameResolver.Provider.ShouldBeOfType<EntityTableNameProvider>();
    }
}
