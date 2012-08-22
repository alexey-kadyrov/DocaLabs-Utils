using System;
using System.Threading;
using DocaLabs.Utils.Clock;

namespace DocaLabs.Testing.Clock
{
    /// <summary>
    /// Implementation of IClockProvider interface which return fixed time.
    /// </summary>
    public class FixedClockProvider : IClockProvider
    {
        ReaderWriterLockSlim Locker { get; set; }
        DateTime Clock { get; set; }

        /// <summary>
        /// Gets/sets the fixed time to be returned as the current time.
        /// </summary>
        public DateTime ClockTime
        {
            get
            {
                try
                {
                    Locker.EnterReadLock();

                    return Clock;
                }
                finally
                {
                    Locker.ExitReadLock();
                }
            }

            set
            {
                try
                {
                    Locker.EnterWriteLock();
                    Clock = value;
                }
                finally
                {
                    Locker.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Initializes an instance of the FixedClockProvider class.
        /// </summary>
        public FixedClockProvider()
        {
            Locker = new ReaderWriterLockSlim();
            Clock = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets a DateTime object that is set to the fixed time.
        /// </summary>
        /// <returns>An object whose value is the current date and time.</returns>
        public DateTime GetCurrentTime()
        {
            return ClockTime;
        }
    }
}
