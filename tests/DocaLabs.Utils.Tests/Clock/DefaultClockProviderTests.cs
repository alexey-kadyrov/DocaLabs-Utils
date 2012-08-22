using System;
using DocaLabs.Utils.Clock;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Utils.Tests.Clock
{
    class default_clock_provider
    {
        static IClockProvider clock_provider;
        static DateTime result;

        Establish context = 
            () => clock_provider = new DefaultClockProvider();

        Because of =
            () => result = clock_provider.GetCurrentTime();

        It should_return_time_in_utc =
            () => result.Kind.ShouldEqual(DateTimeKind.Utc);

        It should_return_time_close_to_current_system_time =
            () => result.ShouldBeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
