using System;

namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Defines methods to get IRequestSerialization for an http client and query. All public methods are thread safe.
    /// </summary>
    public static class RequestSerializationFactory
    {
        /// <summary>
        /// Gets IRequestSerialization for an http client and query.
        /// Looks for RequestSerializationAttribute descendants defined on:
        ///     1. Query class level
        ///     2. One of it's properties
        ///     3. HttpClient level
        /// </summary>
        public static IRequestSerialization GetSerializer(object httpClient, object query)
        {
            if(httpClient == null)
                throw new ArgumentNullException("httpClient");

            if (query != null)
            {
                var serializer = TryQueryClassLevel(query);
                if (serializer != null)
                    return serializer;

                serializer = TryQueryPropertyLevel(query);
                if (serializer != null)
                    return serializer;
            }

            return TryHttpClientClassLevel(httpClient);
        }

        static IRequestSerialization TryQueryClassLevel(object query)
        {
            var attrs = query.GetType().GetCustomAttributes(typeof(RequestSerializationAttribute), true);

            return attrs.Length == 0 
                ? null 
                : attrs[0] as IRequestSerialization;
        }

        static IRequestSerialization TryQueryPropertyLevel(object query)
        {
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (var property in query.GetType().GetProperties())
            {
                var attrs = property.GetCustomAttributes(typeof(RequestSerializationAttribute), true);
                if (attrs.Length <= 0)
                    continue;

                return attrs[0] as IRequestSerialization;
            }

            return null;
            // ReSharper restore LoopCanBeConvertedToQuery
        }

        static IRequestSerialization TryHttpClientClassLevel(object httpClient)
        {
            var attrs = httpClient.GetType().GetCustomAttributes(typeof(RequestSerializationAttribute), true);

            return attrs.Length == 0 
                ? null 
                : attrs[0] as IRequestSerialization;
        }
    }
}
