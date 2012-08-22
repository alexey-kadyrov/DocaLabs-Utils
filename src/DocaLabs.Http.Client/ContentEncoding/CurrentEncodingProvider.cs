namespace DocaLabs.Http.Client.ContentEncoding
{
    /// <summary>
    /// Holds current implementation of the IEncodingProvider interface.
    /// </summary>
    public static class CurrentEncodingProvider
    {
        static volatile IEncodingProvider _current;

        /// <summary>
        /// Gets/sets current IEncodingProvider implementation.
        /// Setting the property to null will force to return the DefaultEncodingProvider next time the getter is called.
        /// The getter will never return null value.
        /// </summary>
        public static IEncodingProvider Current
        {
            get { return _current ?? DefaultLazyProvider.LazyProvider; }
            set { _current = value; }
        }

        static class DefaultLazyProvider
        {
            internal static IEncodingProvider LazyProvider { get; private set; }

            static DefaultLazyProvider()
            {
                LazyProvider = new DefaultEncodingProvider();
            }
        }
    }
}
