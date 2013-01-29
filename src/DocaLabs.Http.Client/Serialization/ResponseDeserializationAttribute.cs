using System;

namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Defines base class for attributes that are used to deserialize objects from a web response stream.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public abstract class ResponseDeserializationAttribute : Attribute, IResponseDeserialization
    {
        /// <summary>
        /// When is overridden in derived class it deserializes an object from the web response.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize.</typeparam>
        /// <param name="response">Web response to deserialize from.</param>
        /// <returns>Deserialized object.</returns>
        public abstract T Deserialize<T>(HttpResponse response);
    }
}
