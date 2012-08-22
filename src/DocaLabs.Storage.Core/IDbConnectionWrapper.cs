using System;
using System.Data.Common;

namespace DocaLabs.Storage.Core
{
    /// <summary>
    /// Holds an instance of the DbConnection class.
    /// The implementations must dispose wrapped connections when being disposed themselves.
    /// </summary>
    public interface IDbConnectionWrapper : IDisposable
    {
        /// <summary>
        /// Gets wrapped instance of the DbConnection class.
        /// </summary>
        DbConnection Connection { get; }
    }
}
