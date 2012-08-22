using System;

namespace DocaLabs.Utils.Clock
{
    /// <summary>
    /// Holds current implementation of the IClockProvider interface.
    /// </summary>
    public static class CurrentClock
    {
        static volatile IClockProvider _provider;

        /// <summary>
        /// Gets/sets current IClockProvider implementation.
        /// Setting the property to null will force to return the DefaultClockProvider next time the getter is called.
        /// The getter will never return null value.
        /// </summary>
        public static IClockProvider Provider
        {
            get { return _provider ?? DefaultLazyClockProvider.LazyProvider; }
            set { _provider = value; }
        }

        /// <summary>
        /// Gets a DateTime object that is set to the current date and time on this computer, expressed as UTC using the current IClockProvider implementation.
        /// </summary>
        public static DateTime UtcNow { get { return Provider.GetCurrentTime(); } }

        static class DefaultLazyClockProvider
        {
            internal static IClockProvider LazyProvider { get; private set; }

            static DefaultLazyClockProvider()
            {
                LazyProvider = new DefaultClockProvider();
            }
        }
    }
}
