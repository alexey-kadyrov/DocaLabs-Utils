using System;

namespace DocaLabs.Http.Client.Mapping
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class QueryParameterAttribute : Attribute
    {
        public string Name { get; set; }
        public string Format { get; set; }

        public QueryParameterAttribute(string parameterName)
        {
            Name = parameterName;
        }
    }
}
