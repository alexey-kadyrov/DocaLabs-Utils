﻿using System;
using Newtonsoft.Json;

namespace DocaLabs.Http.Client.Deserialization
{
    /// <summary>
    /// Deserializes JSON object from the web response.
    /// </summary>
    public class DeserializeFromJsonAttribute : ResponseDeserializationAttribute
    {
        /// <summary>
        /// Deserializes JSON object from the web response.
        /// </summary>
        public override object Deserialize(HttpResponse response, Type resultType)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            if (resultType == null)
                throw new ArgumentNullException("resultType");

            var s = response.AsString();

            try
            {
                return string.IsNullOrWhiteSpace(s)
                    ? null
                    : JsonConvert.DeserializeObject(s, resultType);
            }
            catch (Exception e)
            {
                throw new UnrecoverableHttpClientException(e.Message, e);
            }
        }
    }
}