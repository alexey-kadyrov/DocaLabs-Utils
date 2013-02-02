namespace DocaLabs.Http.Client.JsonSerialization
{
    /// <summary>
    /// Defines methods to serialize an object using JSON notation.
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Serializes an object into string using JSON notation.
        /// </summary>
        string Serialize(object obj);
    }
}
