using System;
using DocaLabs.Utils.Conversion;

namespace DocaLabs.Http.Client.Deserialization
{
    /// <summary>
    /// Deserializes the response stream as plain string and then converts to the resulting type.
    /// </summary>
    public class PlainTextResponseDeserializer : IResponseDeserializationProvider
    {
        /// <summary>
        /// Deserializes the response stream as plain string and then converts to the resulting type.
        /// </summary>
        public TResult Deserialize<TResult>(HttpResponse response)
        {
            try
            {
                return CustomConverter.Current.ChangeType<TResult>(response.AsString());
            }
            catch (Exception e)
            {
                throw new UnrecoverableHttpClientException(e.Message, e);
            }
        }

        /// <summary>
        /// Returns true if the content type is 'text/plain' and the TResult is "simple type", like int, string, Guid, double, etc.
        /// or if the content type is 'text/html' and the TResult is string.
        /// </summary>
        public bool CheckIfSupports<TResult>(HttpResponse response)
        {
            return
                (!string.IsNullOrWhiteSpace(response.ContentType)) &&
                    (
                        (
                            string.Compare(response.ContentType, "text/plain", StringComparison.OrdinalIgnoreCase) == 0 &&
                            typeof(TResult).IsSimpleType()
                        ) ||
                        (
                            string.Compare(response.ContentType, "text/html", StringComparison.OrdinalIgnoreCase) == 0 &&
                            typeof(TResult) == typeof(string)
                        )
                    );
        }
    }
}
