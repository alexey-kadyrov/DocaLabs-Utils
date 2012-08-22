using System.Collections.Generic;

namespace DocaLabs.Http.Client.Mapping
{
    public interface IParsedProperty
    {
        IEnumerable<KeyValuePair<string, IList<string>>> GetValue(object obj);
    }
}
