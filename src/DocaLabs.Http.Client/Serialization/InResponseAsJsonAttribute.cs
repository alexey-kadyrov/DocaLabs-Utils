namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Deserializes json serialized object from the web response.
    /// </summary>
    public class InResponseAsJsonAttribute : InResponseAttribute
    {
        /// <summary>
        /// Deserializes json serialized object from the web response.
        /// </summary>
        public override T Deserialize<T>(HttpResponse response)
        {
            return response.AsJsonObject<T>();
        }
    }
}
