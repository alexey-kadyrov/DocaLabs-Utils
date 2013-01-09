using System;
using DocaLabs.Utils.Clock;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace DocaLabs.Utils.Tests.Clock
{
    [Subject(typeof(CurrentClock))]
    class when_utcnow_on_current_clock_is_called
    {
        static DateTime target_time;
        static Mock<IClockProvider> mock_provider;

        Cleanup after_each = () => CurrentClock.Provider = null;

        Establish context = () =>
        {
            target_time = new DateTime(2011, 12, 17, 16, 40, 55, 250);
            
            mock_provider = new Mock<IClockProvider>();
            mock_provider.Setup(x => x.GetCurrentTime()).Returns(target_time);
        };

        Because of = 
            () => CurrentClock.Provider = mock_provider.Object;

        It should_return_value_supplied_by_clock_provider =
            () => CurrentClock.UtcNow.ShouldEqual(target_time);
    }

    [Subject(typeof(CurrentClock))]
    class current_clock_in_default_state
    {
        It should_have_non_null_provider = 
            () => CurrentClock.Provider.ShouldNotBeNull();

        It should_use_default_clock_provider = 
            () => CurrentClock.Provider.ShouldBeOfType<DefaultClockProvider>();
    }

    [Subject(typeof(CurrentClock))]
    class when_provider_on_current_clock_is_set_to_null
    {
        Cleanup after_each = 
            () => CurrentClock.Provider = null;

        Establish context = 
            () => CurrentClock.Provider = new Mock<IClockProvider>().Object;

        Because of = 
            () => CurrentClock.Provider = null;

        It getter_should_return_non_null_provider = 
            () => CurrentClock.Provider.ShouldNotBeNull();

        It should_use_default_clock_provider = 
            () => CurrentClock.Provider.ShouldBeOfType<DefaultClockProvider>();
    }
}
