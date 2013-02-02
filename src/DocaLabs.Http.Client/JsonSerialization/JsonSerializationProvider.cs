using System;

namespace DocaLabs.Http.Client.JsonSerialization
{
    /// <summary>
    /// Provides implementations for serialization objects in JSON notation. All public methods and properties are thread safe.
    /// </summary>
    public static class JsonSerializationProvider
    {
        static readonly object Locker;
        static IJsonSerializer _serializer;
        static IJsonDeserializer _deserializer;

        /// <summary>
        /// Gets or sets the json serializer implementation. The property cannot be set to null.
        /// </summary>
        public static IJsonSerializer Serializer
        {
            get
            {
                lock (Locker)
                {
                    return _serializer;
                }
            }

            set
            {
                if(value == null)
                    throw new ArgumentNullException("value");

                lock (Locker)
                {
                    _serializer = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the json deserializer implementation. The property cannot be set to null.
        /// </summary>
        public static IJsonDeserializer Deserializer
        {
            get
            {
                lock (Locker)
                {
                    return _deserializer;
                }
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                lock (Locker)
                {
                    _deserializer = value;
                }
            }
        }

        static JsonSerializationProvider()
        {
            Locker = new object();
            _serializer = new DefaultJsonSerializer();
            _deserializer = new DefaultJsonDeserializer();
        }
    }
}
