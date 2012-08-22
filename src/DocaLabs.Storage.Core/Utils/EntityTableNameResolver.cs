namespace DocaLabs.Storage.Core.Utils
{
    /// <summary>
    /// Holds current implementation of the IEntityTableNameProvider interface.
    /// </summary>
    public static class EntityTableNameResolver
    {
        static volatile IEntityTableNameProvider _provider;

        /// <summary>
        /// Gets or sets current IEntityTableNameProvider implementation.
        /// Setting the property to null will force to return the EntityTableNameProvider next time the getter is called.
        /// The getter will never return null value.
        /// </summary>
        public static IEntityTableNameProvider Provider
        {
            get { return _provider ?? DefaultLazyProvider.LazyProvider; }
            set { _provider = value; }
        }

        /// <summary>
        /// Resolves the type to a table name. Uses the current provide to do the job.
        /// </summary>
        /// <typeparam name="TEntity">Type to be resolved.</typeparam>
        /// <returns>Table name.</returns>
        public static string Resolve<TEntity>()
        {
            return Provider.Resolve<TEntity>();
        }

        static class DefaultLazyProvider
        {
            internal static IEntityTableNameProvider LazyProvider { get; private set; }

            static DefaultLazyProvider()
            {
                LazyProvider = new EntityTableNameProvider();
            }
        }
    }
}
