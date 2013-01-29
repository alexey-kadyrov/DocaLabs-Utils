using System.Net;

namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Defines helper methods to deserialize the web response.
    /// </summary>
    public static class ResponseParser
    {
        /// <summary>
        /// Gets the web response and tries to deserialize the response. It tries to do:
        ///     1. To get ResponseDeserializationAttribute if defined on TResult class.
        ///     2. Checks whenever the response is application/json
        ///     3. Checks whenever the response is text/xml
        /// </summary>
        public static TResult Parse<TResult>(this WebRequest request)
        {
            using (var response = new HttpResponse(request.GetResponse()))
            {
                return TransformResult<TResult>(response);
            }
        }

        static TResult TransformResult<TResult>(HttpResponse response)
        {
            var attrs = typeof(TResult).GetCustomAttributes(typeof(ResponseDeserializationAttribute), true);
            if (attrs.Length > 0)
                return ((ResponseDeserializationAttribute)attrs[0]).Deserialize<TResult>(response);

            if (response.IsJson())
                return response.AsJsonObject<TResult>();

            if (response.IsXml())
                return response.AsXmlObject<TResult>();

            throw new HttpClientException(Resources.Text.cannot_figure_out_how_to_deserialize)
            {
                DoNotRetry = true
            };
        }

    }
}
