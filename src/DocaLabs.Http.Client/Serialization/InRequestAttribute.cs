using System;
using System.Net;

namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Defines base class for attributes that are used to serialize objects into a web request stream.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public abstract class InRequestAttribute : Attribute
    {
        /// <summary>
        /// When is overridden in derived class it serializes a given object into the web request.
        /// </summary>
        /// <param name="obj">Object to be serialized.</param>
        /// <param name="request">Web request where to serialize to.</param>
        public abstract void Serialize(object obj, WebRequest request);
    }
}
