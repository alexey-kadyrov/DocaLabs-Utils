using System.Collections.Generic;
using System.Reflection;
using DocaLabs.Utils;

namespace DocaLabs.Http.Client.Mapping
{
    public class ParsedObjectProperty : ParsedPropertyBase, IParsedProperty
    {
        ParsedObjectProperty(PropertyInfo info)
            : base(info)
        {
        }

        public static IParsedProperty TryParse(PropertyInfo info)
        {
            return info.PropertyType.IsPrimitive
                ? new ParsedObjectProperty(info)
                : null;
        }

        public IEnumerable<KeyValuePair<string, IList<string>>> GetValue(object obj)
        {
            IEnumerable<KeyValuePair<string, IList<string>>> values = null;

            var value = Info.GetValue(obj, null);

            if (value != null)
            {
                var customeMapper = obj as ICustomQueryMapper;
                if (customeMapper != null)
                    values = customeMapper.ToParameterDictionary();
            }

            return values ?? new CustomNameValueCollection();
        }
    }
}
