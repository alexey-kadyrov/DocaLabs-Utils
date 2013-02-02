﻿using System;

namespace DocaLabs.Http.Client.JsonSerialization
{
    /// <summary>
    /// Defines methods to deserialize an object from string in JSON notation.
    /// </summary>
    public interface IJsonDeserializer
    {
        /// <summary>
        /// Deserializes an object from string in JSON notation.
        /// </summary>
        object Deserialize(string value, Type type);
    }
}