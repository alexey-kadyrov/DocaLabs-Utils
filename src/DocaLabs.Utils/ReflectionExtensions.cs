using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DocaLabs.Utils
{
    public static class ReflectionExtensions
    {
        public static bool IsValidOn(this CustomAttributeData data, AttributeTargets flags)
        {
            var attributes = data.AttributeType.GetCustomAttributes(typeof(AttributeUsageAttribute), true);

            return attributes.Length == 0 || attributes.Any(x => ((AttributeUsageAttribute)x).ValidOn.HasFlag(flags));
        }

        public static IList<MethodInfo> GetAllMethods(this Type type, BindingFlags flags)
        {
            flags |= BindingFlags.FlattenHierarchy;

            var list = type.GetMethods(flags).Where(x => !x.IsSpecialName).ToList();

            if(!type.IsInterface)
                return list;

            foreach (var @interface in type.GetInterfaces())
            {
                var methods = @interface.GetMethods(flags).Where(x => (!x.IsSpecialName) && (!list.Exists(x)));

                list.AddRange(methods);
            }

            return list;
        }

        public static bool Exists(this IEnumerable<MethodInfo> collection, MethodInfo method)
        {
            var name = method.Name;
            var genericArguments = method.IsGenericMethod ? method.GetGenericArguments() : null;
            var parameters = method.GetParameters();

            foreach (var existingMethod in collection)
            {
                if (existingMethod.Name == name && existingMethod.GetParameters().Compare(parameters))
                {
                    if (genericArguments == null)
                    {
                        if ((!existingMethod.IsGenericMethod))
                            return true;
                    }
                    else if (existingMethod.IsGenericMethod && existingMethod.GetGenericArguments().Compare(genericArguments))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static IList<PropertyInfo> GetAllProperties(this Type type, BindingFlags flags)
        {
            flags |= BindingFlags.FlattenHierarchy;

            var list = type.GetProperties(flags).ToList();

            if (!type.IsInterface)
                return list;

            foreach (var @interface in type.GetInterfaces())
            {
                var properties = @interface.GetProperties(flags).Where(x => (!x.IsSpecialName) && (!list.Exists(x)));

                list.AddRange(properties);
            }

            return list;
        }

        public static bool Exists(this IEnumerable<PropertyInfo> collection, PropertyInfo property)
        {
            var name = property.Name;
            var propertType = property.PropertyType;
            var parameters = property.GetIndexParameters();

            return collection.Any(
                    existingProperty => existingProperty.Name == name 
                    && existingProperty.PropertyType == propertType 
                    && existingProperty.GetIndexParameters().Compare(parameters));
        }

        static bool Compare(this ICollection<ParameterInfo> left, IList<ParameterInfo> right)
        {
            if (left.Count != right.Count)
                return false;

            return !left.Where((t, i) => t.ParameterType != right[i].ParameterType).Any();
        }

        static bool Compare(this ICollection<Type> left, IList<Type> right)
        {
            if (left.Count != right.Count)
                return false;

            return !left.Where((t, i) => t != right[i]).Any();
        }
    }
}
