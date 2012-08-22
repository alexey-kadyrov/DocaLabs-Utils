using System;
using Machine.Specifications;

namespace DocaLabs.Testing.Common.MSpec
{
    public static class ShouldThrow
    {
        public static T Exception<T>(Action throwingAction) where T : Exception
        {
            try
            {
                throwingAction();
            }
            catch (T exception)
            {
                return exception;
            }

           throw new SpecificationException(string.Format("Expected {0} to be thrown.", typeof(T)));
        }

        public static T WithMessage<T>(this T exception, Action<string> shouldSatisfy) where T : Exception
        {
            shouldSatisfy(exception.Message);

            return exception;
        }

        public static T Satisfying<T>(this T exception, Action<T> condition) where T : Exception
        {
            condition(exception);
            return exception;
        }
    }
}
