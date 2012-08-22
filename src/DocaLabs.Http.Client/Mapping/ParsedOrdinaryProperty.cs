﻿using System.Collections.Generic;
using System.Reflection;
using DocaLabs.Utils;
using DocaLabs.Utils.Conversion;

namespace DocaLabs.Http.Client.Mapping
{
    public class ParsedOrdinaryProperty : ParsedPropertyBase, IParsedProperty
    {
        ParsedOrdinaryProperty(PropertyInfo info)
            : base(info)
        {
        }

        public static IParsedProperty TryParse(PropertyInfo info)
        {
            var propertyType = info.PropertyType;

            return propertyType.IsPrimitive || propertyType == typeof(string) || propertyType == typeof(byte[])
                ? new ParsedOrdinaryProperty(info) 
                : null;
        }

        public IEnumerable<KeyValuePair<string, IList<string>>> GetValue(object obj)
        {
            // ReSharper disable FormatStringProblem
            var values = new CustomNameValueCollection();

            var value = Info.GetValue(obj, null);

            if (value != null)
            {
                values.Add(Name, string.IsNullOrWhiteSpace(Format)
                                    ? CustomConverter.Current.ChangeType<string>(value)
                                    : string.Format("{0:" + Format + "}", value));
            }

            return values;
            // ReSharper restore FormatStringProblem
        }
    }
}
