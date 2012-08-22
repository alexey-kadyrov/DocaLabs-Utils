using System;

namespace DocaLabs.Utils.Clock
{
    /// <summary>
    /// Defines the methods that are required to get current time expressed as UTC.
    /// Introduced mainly to be used in tests.
    /// </summary>
    public interface IClockProvider
    {
        /// <summary>
        /// Gets a DateTime object that is set to the current date and time on this computer, expressed as UTC.
        /// </summary>
        /// <returns>An object whose value is the current UTC date and time.</returns>
        DateTime GetCurrentTime();
    }
}
