using System;

namespace DocaLabs.Http.Client.Mapping.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class QueryIgnoreAttribute : Attribute
    {
    }
}
