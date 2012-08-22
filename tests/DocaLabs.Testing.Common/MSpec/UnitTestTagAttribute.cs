using System;
using Machine.Specifications;

namespace DocaLabs.Testing.Common.MSpec
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class UnitTestTagAttribute : TagsAttribute
    {
        public UnitTestTagAttribute()
            : base(CommonTestTags.UnitTest)
        {
        }
    }
}
