using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DocaLabs.Http.Client.Mapping.Attributes;
using DocaLabs.Utils;

namespace DocaLabs.Http.Client.Mapping
{
    /// <summary>
    /// Defines class that contains information about properties that can be mapped.
    /// </summary>
    public class ParsedType
    {
        /// <summary>
        /// Gets parsed properties.
        /// </summary>
        public IEnumerable<IParsedProperty> Properties { get; private set; }

        ParsedType(Type type)
        {
            Properties = Parse(type);
        }

        /// <summary>
        /// Initializes a new instance of the ParsedType class for the specified type.
        /// </summary>
        public static ParsedType ParseType(Type type)
        {
            return new ParsedType(type);
        }

        static IEnumerable<IParsedProperty> Parse(Type type)
        {
            return type.IsPrimitive
                ? new List<IParsedProperty>()
                : type.GetAllProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(ParseProperty)
                    .Where(x => x != null)
                    .ToList();
        }

        static IParsedProperty ParseProperty(PropertyInfo info)
        {
            if (Ignore(info))
                return null;

            return TryGetCustomPropertyParser(info)
                ?? ParsedCollectionProperty.TryParse(info)
                ?? ParsedOrdinaryProperty.TryParse(info)
                ?? ParsedObjectProperty.TryParse(info);
        }

        static bool Ignore(PropertyInfo info)
        {
            // We don't do indexers, as in general it's impossible to guess what would be the required index parameters
            return  info.GetIndexParameters().Length > 0 ||
                    info.GetGetMethod() == null ||
                    info.GetCustomAttributes(typeof(QueryIgnoreAttribute), true).Length > 0;
        }

        static IParsedProperty TryGetCustomPropertyParser(PropertyInfo info)
        {
            var attrs = info.GetCustomAttributes(typeof (QueryPropertyParserAttribute), true);
            return attrs.Length > 0
                ? ((QueryPropertyParserAttribute)attrs[0]).GetParser(info)
                : null;
        }
    }
}
