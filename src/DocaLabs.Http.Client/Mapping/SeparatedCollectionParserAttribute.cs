using System.Reflection;

namespace DocaLabs.Http.Client.Mapping
{
    public class SeparatedCollectionParserAttribute : QueryPropertyParserAttribute
    {
        public char Separator { get; set; }

        public override IParsedProperty GetParser(PropertyInfo info)
        {
            return new SeparatedCollectionParser(info)
            {
                Separator = Separator
            };
        }
    }
}
