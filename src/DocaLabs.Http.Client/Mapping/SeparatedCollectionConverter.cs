using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DocaLabs.Utils;
using DocaLabs.Utils.Conversion;

namespace DocaLabs.Http.Client.Mapping
{
    public class SeparatedCollectionConverter : ParsedPropertyBase, IConvertProperty
    {
        public char Separator { get; set; }

        public SeparatedCollectionConverter(PropertyInfo info)
            : base(info)
        {
            Separator = '|';
        }

        public IEnumerable<KeyValuePair<string, IList<string>>> GetValue(object obj)
        {
            var values = new CustomNameValueCollection();

            var collection = Info.GetValue(obj, null) as IEnumerable;

            if (collection != null)
            {
                var stringBuilder = new StringBuilder();

                foreach (var value in collection)
                {
                    if (stringBuilder.Length >= 0)
                        stringBuilder.Append(Separator);

                    stringBuilder.Append(CustomConverter.Current.ChangeType<string>(value));
                }

                if(stringBuilder.Length > 0)
                    values.Add(Name, stringBuilder.ToString());
            }

            return values;
        }
    }
}
