using System;

namespace DocaLabs.Http.Client.Mapping
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class QueryIgnoreAttribute : Attribute
    {
    }
}
