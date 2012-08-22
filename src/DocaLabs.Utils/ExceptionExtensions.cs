using System;
using System.Collections.Generic;
using System.Linq;

namespace DocaLabs.Utils
{
    /// <summary>
    /// Exception extensions.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Tries to find all instances of the specified exception type in the inner exception chain and in Aggregate Exception chain.
        /// The root exception is tested as well.
        /// </summary>
        /// <typeparam name="T">Exception type to find.</typeparam>
        /// <param name="exception">The root exception where to start looking for.</param>
        /// <returns>List of unique found instances, if nothing is found then the list will be empty but never null, even for null input value.</returns>
        public static ICollection<T> Find<T>(this Exception exception) where T : Exception
        {
            var result = new HashSet<T>();

            if (exception == null)
                return result;

            TrySelf(exception, result);

            TryFindInInnerException(exception, result);

            TryFindInAggregateException(exception, result);

            return result;
        }

        static void TrySelf<T>(Exception exception, ICollection<T> result) where T : Exception
        {
            var targetException = exception as T;
            if (targetException != null)
                result.Add(targetException);
        }

        static void TryFindInInnerException<T>(Exception exception, ICollection<T> result) where T : Exception
        {
            foreach (var e in exception.InnerException.Find<T>())
                result.Add(e);
        }

        static void TryFindInAggregateException<T>(Exception exception, ICollection<T> result) where T : Exception
        {
            var aggregate = exception as AggregateException;

            if (aggregate == null)
                return;

            foreach (var e in aggregate.InnerExceptions.SelectMany(inner => inner.Find<T>()))
                result.Add(e);
        }
    }
}
