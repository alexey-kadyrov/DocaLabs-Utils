using System;
using System.Diagnostics;
using System.Threading;
using DocaLabs.Utils.Clock;

namespace DocaLabs.Testing.Clock
{
    /// <summary>
    /// Implementation of IClockProvider interface which can speed up the time.
    /// </summary>
    public class SpeedyClockProvider : IClockProvider
    {
        DateTime _initialTime;

        ReaderWriterLockSlim Locker { get; set; }
        Stopwatch Stopwatch { get; set; }
        long Speed { get; set; }

        /// <summary>
        /// Gets/sets the base time which is used to calculate the current time.
        /// </summary>
        public DateTime InitialTime
        {
            get
            {
                try
                {
                    Locker.EnterReadLock();
                    return _initialTime;
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
                    _initialTime = value;
                    Stopwatch = Stopwatch.StartNew();
                }
                finally
                {
                    Locker.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Initializes the instance of the SpeedyClockProvider class with the specified speed.
        /// </summary>
        /// <param name="speed">The multiplier speed (for example 2 will means that the clock will run twice of the normal time).</param>
        public SpeedyClockProvider(long speed)
        {
            Locker = new ReaderWriterLockSlim();
            Speed = speed;
            InitialTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets a DateTime object that is set to the speeded up current date.
        /// </summary>
        /// <returns>An object whose value is the current date and time.</returns>
        public DateTime GetCurrentTime()
        {
            try
            {
                Locker.EnterReadLock();
                return _initialTime.AddMilliseconds(Stopwatch.ElapsedMilliseconds * Speed);
            }
            finally
            {
                Locker.ExitReadLock();
            }
        }
    }
}
