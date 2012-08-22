using System;
using Machine.Specifications;

namespace DocaLabs.Testing.Common.MSpec
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class IntegrationTagAttribute : TagsAttribute
    {
        public IntegrationTagAttribute()
            : base(CommonTestTags.Integration)
        {
        }
    }
}
