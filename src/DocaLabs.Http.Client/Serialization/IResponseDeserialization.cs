namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Defines methods that are used to deserialize objects from a web response stream.
    /// </summary>
    public interface IResponseDeserialization
    {
        /// <summary>
        /// When is overridden in derived class it deserializes an object from the web response.
        /// </summary>
        /// <typeparam name="TResult">Type of the object to deserialize.</typeparam>
        /// <param name="response">Web response to deserialize from.</param>
        /// <returns>Deserialized object.</returns>
        TResult Deserialize<TResult>(HttpResponse response);
    }
}