using System;

namespace DocaLabs.Http.Client
{
    /// <summary>
    /// Extension utilities.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns true if the type is primitive or string/decimal/Guid/dateTime/TimeSpan/DateTimeOffset.
        /// </summary>
        public static bool IsSimpleType(this Type type)
        {
            return (type == typeof(string) ||
                    type.IsPrimitive ||
                    type == typeof(decimal) ||
                    type == typeof(Guid) ||
                    type == typeof(DateTime) ||
                    type == typeof(TimeSpan) ||
                    type == typeof(DateTimeOffset) ||
                    type.IsEnum);
        }
    }
}
