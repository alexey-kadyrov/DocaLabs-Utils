using System.Reflection;
using DocaLabs.Http.Client.Mapping.Attributes;

namespace DocaLabs.Http.Client.Mapping
{
    public abstract class ParsedPropertyBase
    {
        protected PropertyInfo Info { get; private set; }
        protected string Name { get; private set; }
        protected string Format { get; private set; }

        protected ParsedPropertyBase(PropertyInfo info)
        {
            Info = info;

            var attrs = info.GetCustomAttributes(typeof (QueryParameterAttribute), true);
            if(attrs.Length > 0)
            {
                Name = ((QueryParameterAttribute) attrs[0]).Name;
                Format = ((QueryParameterAttribute)attrs[0]).Format;
            }
            else
            {
                Name = Info.Name;
            }

        }
    }
}
