namespace DocaLabs.Utils.Configuration
{
    /// <summary>
    /// Holds current implementation of the IConfigurationManager interface.
    /// </summary>
    public static class CurrentConfigurationManager
    {
        static volatile IConfigurationManager _current;

        /// <summary>
        /// Gets/sets current IConfigurationManager implementation.
        /// Setting the property to null will force to return the DefaultConfigurationManager next time the getter is called.
        /// The getter will never return null value.
        /// </summary>
        public static IConfigurationManager Current
        {
            get { return _current ?? DefaultLazyConfigurationManager.LazyManager; }
            set { _current = value; }
        }

        static class DefaultLazyConfigurationManager
        {
            internal static IConfigurationManager LazyManager { get; private set; }

            static DefaultLazyConfigurationManager()
            {
                LazyManager = new DefaultConfigurationManager();
            }
        }
    }
}
