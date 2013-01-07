
namespace MongoDB.Driver.Security.Mechanisms.Sspi
{
    /// <summary>
    /// This is represented as a string in AcquireCredentialsHandle. This value will have .ToString() called on it.
    /// </summary>
    internal enum SspiPackage
    {
        /// <summary>
        /// Kerberos
        /// </summary>
        Kerberos
    }
}
