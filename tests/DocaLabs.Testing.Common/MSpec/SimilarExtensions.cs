using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace DocaLabs.Testing.Common.MSpec
{
    /// <remarks>
    /// It's impossible to redefine the equality behaviour for objects as it will mess the entity tracking
    /// in the DataServiceContext which uses Dictionary and expects the default behaviour for
    /// reference types - comparing references.
    /// The dictionary uses hash code first to assign bucket and if there are more than one object 
    /// with the same hash it will compare those objects.
    /// If the equality is redefined the way that it compares properties of the object the DataServiceContext
    /// won't be able to detect that it's already tracking the entity as the dictionary will look for the
    /// changed object in different bucket.
    /// By convention if the Equals is redefined the GetHaskCode must be redefined as well in the way
    /// that it must always return the same value for equal objects.
    /// The good article is: http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx
    /// </remarks>
    public static class SimilarExtensions
    {
        public static void ShouldContainOnlySimilar<TActual, TExpected>(this IEnumerable<TActual> list, Func<TActual, TExpected, bool> comparer, params TExpected[] items)
        {
            var count = 0L;

            var message = new StringBuilder();

            foreach (var actual in list)
            {
                if (!items.Contains(actual, comparer))
                {
                    message
                        .Append("Should contain element: ")
                        .Append(actual)
                        .Append("\n");
                }

                count++;
            }

            if (count != items.Length)
            {
                message
                    .Append("Should contain ")
                    .Append(items.Length)
                    .Append(" elements but contains ")
                    .Append(count)
                    .Append(" elements.\n");
            }

            if (message.Length > 0)
                throw new SpecificationException(message.ToString());
        }

        public static void ShouldContainOnlySimilar<TActual, TExpected>(this IEnumerable list, Func<TActual, TExpected, bool> comparer, params TExpected[] items)
        {
            var count = 0L;

            var message = new StringBuilder();

            foreach (var actual in list)
            {
                if (!items.Contains((TActual)actual, comparer))
                {
                    message
                        .Append("Should contain element: ")
                        .Append(actual)
                        .Append("\n");
                }

                count++;
            }

            if (count != items.Length)
            {
                message
                    .Append("Should contain ")
                    .Append(items.Length)
                    .Append(" elements but contains ")
                    .Append(count)
                    .Append(" elements.\n");
            }

            if (message.Length > 0)
                throw new SpecificationException(message.ToString());
        }

        public static void ShouldContainOnlySimilar<TActual, TExpected>(this IEnumerator<TActual> enumerator, Func<TActual, TExpected, bool> comparer, params TExpected[] items)
        {
            var count = 0L;

            var message = new StringBuilder();

            while (enumerator.MoveNext())
            {
                if (!items.Contains(enumerator.Current, comparer))
                {
                    message
                        .Append("Should contain element: ")
                        .Append(enumerator.Current)
                        .Append("\n");
                }

                count++;
            }

            if (count != items.Length)
            {
                message
                    .Append("Should contain ")
                    .Append(items.Length)
                    .Append(" elements but contains ")
                    .Append(count)
                    .Append(" elements.\n");
            }

            if (message.Length > 0)
                throw new SpecificationException(message.ToString());
        }

        public static void ShouldContainOnlySimilar(this IEnumerator enumerator, Func<object, object, bool> comparer, params object[] items)
        {
            var count = 0L;

            var message = new StringBuilder();

            while (enumerator.MoveNext())
            {
                if (!items.Contains(enumerator.Current, comparer))
                {
                    message
                        .Append("Should contain element: ")
                        .Append(enumerator.Current)
                        .Append("\n");
                }

                count++;
            }

            if (count != items.Length)
            {
                message
                    .Append("Should contain ")
                    .Append(items.Length)
                    .Append(" elements but contains ")
                    .Append(count)
                    .Append(" elements.\n");
            }

            if (message.Length > 0)
                throw new SpecificationException(message.ToString());
        }

        public static void ShouldBeSimilar<TActual, TExpected>(this TActual actual, TExpected expected, Func<TActual, TExpected, bool> comparer)
        {
            if (!comparer(actual ,expected))
                throw new SpecificationException(string.Format("Expected {0} but was {1}", expected, actual));
        }

        public static void ShouldBeSimilar<TActual, TExpected>(this TActual actual, TExpected expected)
        {
            if (Equals(actual, null))
            {
                if(Equals(expected, null))
                    return;

                throw new SpecificationException(string.Format("Expected {0} to be null.", expected));
            }

            if (Equals(expected, null))
                throw new SpecificationException(string.Format("Expected {0} to be null.", actual));

            foreach (var expectedProperty in expected.GetType().GetProperties())
            {
                var actualProperty = actual.GetType().GetProperty(expectedProperty.Name);
                if(actualProperty == null)
                    throw new SpecificationException(string.Format("Expected that the object should support {0} property.", expectedProperty.Name));

                var actualValue = actualProperty.GetValue(actual, null);
                var expectedValue = expectedProperty.GetValue(expected, null);

                if(!Equals(actualValue, expectedValue))
                    throw new SpecificationException(string.Format("Expected {0} should be equal to {1} but was {2}.", actualProperty.Name, expectedValue, actualValue));
            }
        }

        public static void ShouldContainSimilarValue<TActual, TExpected>(this IEnumerable<TActual> items, TExpected expected, Func<TExpected, TActual, bool> comparer)
        {
            if (!items.Contains(expected, comparer))
                throw new SpecificationException(string.Format("Expected to contain {0} but didn't", expected));
        }

        static bool Contains<TCollection, TValue>(this IEnumerable<TCollection> items, TValue value, Func<TValue, TCollection, bool> comparer)
        {
            return items.Any(item => comparer(value, item));
        }
    }
}
