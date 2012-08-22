namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Deserializes xml serialized object from the web response.
    /// </summary>
    public class InResponseAsXmlAttribute : InResponseAttribute
    {
        /// <summary>
        /// Deserializes xml serialized object from the web response.
        /// </summary>
        public override T Deserialize<T>(HttpResponse response)
        {
            return response.AsXmlObject<T>();
        }
    }
}
