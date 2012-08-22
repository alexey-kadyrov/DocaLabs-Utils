using System;
using System.Reflection;

namespace DocaLabs.Http.Client.Mapping
{
    public abstract class QueryPropertyParserAttribute : Attribute
    {
        public abstract IParsedProperty GetParser(PropertyInfo info);
    }
}