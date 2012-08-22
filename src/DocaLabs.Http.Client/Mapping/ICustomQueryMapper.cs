using System.Collections.Generic;

namespace DocaLabs.Http.Client.Mapping
{
    public interface ICustomQueryMapper
    {
        IEnumerable<KeyValuePair<string, IList<string>>> ToParameterDictionary();
    }
}
