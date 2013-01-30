namespace DocaLabs.Http.Client
{
    /// <summary>
    /// Defines empty type which can be used as a generic parameter for HttpClient in place of void.
    /// </summary>
    public struct VoidValue
    {
        /// <summary>
        /// Default value.
        /// </summary>
        public static readonly VoidValue Value = default (VoidValue);
    }
}
