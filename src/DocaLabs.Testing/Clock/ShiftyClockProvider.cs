using System;
using System.Threading;
using DocaLabs.Utils.Clock;

namespace DocaLabs.Testing.Clock
{
    /// <summary>
    /// Implementation of IClockProvider interface which can shift the time by the supplied time span.
    /// </summary>
    public class ShiftyClockProvider : IClockProvider
    {
        ReaderWriterLockSlim Locker { get; set; }
        TimeSpan _timeShift;

        /// <summary>
        /// Gets/sets time shift that should be applied to the current system's time.
        /// </summary>
        public TimeSpan TimeShift
        {
            get
            {
                try
                {
                    Locker.EnterReadLock();
                    return _timeShift;
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
                    _timeShift = value;
                }
                finally
                {
                    Locker.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Initializes an instance of the ShiftyClockProvider class.
        /// </summary>
        public ShiftyClockProvider()
        {
            Locker = new ReaderWriterLockSlim();
            _timeShift = TimeSpan.Zero;
        }

        /// <summary>
        /// Gets a DateTime object that is set to the current date plus shift.
        /// </summary>
        /// <returns>An object whose value is the current date and time.</returns>
        public DateTime GetCurrentTime()
        {
            return DateTime.UtcNow + TimeShift;
        }
    }
}
