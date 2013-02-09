using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;

namespace DocaLabs.Utils
{
    /// <summary>
    /// Provides means to discover alternative implementations (extensions) using MEF.
    /// </summary>
    public static class LibraryExtensionsComposer
    {
        static readonly object Locker;
        static readonly CompositionContainer CompositionContainer;

        /// <summary>
        /// Gets or sets action which is called when the CompositionException is thrown.
        /// </summary>
        public static Action<object, CompositionException> OnCompositionException { get; set; }

        static LibraryExtensionsComposer()
        {
            Locker = new object();

            OnCompositionException = (o,e) => Debug.WriteLine(e.ToString());

            // An aggregate catalogue that combines multiple catalogues
            var catalog = new AggregateCatalog();

            // Adds all the parts found in the base folder where current assembly resolver looks for.
            // Cannot use BaseDirectory as the first choice due that in the web application it will be the parent of the "bin" folder.
            catalog.Catalogs.Add(new DirectoryCatalog(
                    string.IsNullOrWhiteSpace(AppDomain.CurrentDomain.RelativeSearchPath)
                        ? AppDomain.CurrentDomain.BaseDirectory
                        : AppDomain.CurrentDomain.RelativeSearchPath, "DocaLabs.Extensions.*"));

            // Create the CompositionContainer with the parts in the catalogue.
            CompositionContainer = new CompositionContainer(catalog);
        }

        /// <summary>
        /// Fill the imports of the object, they are optional as it expected that all dependencies
        /// can be provided explicitly either through configuration methods or injections
        /// </summary>
        public static void ComposePartsFor(object o)
        {
            try
            {
                lock (Locker)
                {
                    CompositionContainer.ComposeParts(o);
                }
            }
            catch (CompositionException e)
            {
                if (OnCompositionException != null)
                    OnCompositionException(o, e);
            }
        }
    }
}
