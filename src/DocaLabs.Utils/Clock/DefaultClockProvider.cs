using System;

namespace DocaLabs.Utils.Clock
{
    /// <summary>
    /// Default implementation of IClockProvider interface which uses DateTime structure.
    /// </summary>
    public class DefaultClockProvider : IClockProvider
    {
        /// <summary>
        /// Gets a DateTime object that is set to the current date and time on this computer, expressed as UTC.
        /// </summary>
        /// <returns>An object whose value is the current UTC date and time.</returns>
        public DateTime GetCurrentTime()
        {
            return DateTime.UtcNow;
        }
    }
}
