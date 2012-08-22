namespace DocaLabs.Storage.Core
{
    /// <summary>
    /// Defines strong-typed hierarchical blob names.
    /// </summary>
    public interface IBlobName
    {
        /// <summary>
        /// Gets or sets the container's name
        /// </summary>
        string ContainerName { get; set; }

        /// <summary>
        /// Gets the blob name including container
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Parses a hierarchical blob name.
        /// </summary>
        void Parse(string value);
    }
}
