namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Deserializes xml serialized object from the web response.
    /// </summary>
    public class DeserializeFromXmlAttribute : ResponseDeserializationAttribute
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
