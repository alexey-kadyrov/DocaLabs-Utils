using System;

namespace DocaLabs.Http.Client.Tests._Utils
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ClassAttributeWithFieldsAttribute : Attribute
    {
        public string Value;
    }
}