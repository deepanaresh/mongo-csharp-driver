
namespace MongoDB.Driver.Security.Mechanisms.Sspi
{
    /// <summary>
    /// Flags for AcquireCredentialsHandle.
    /// </summary>
    /// <remarks>
    /// See the fCredentialUse at http://msdn.microsoft.com/en-us/library/windows/desktop/aa374712(v=vs.85).aspx.
    /// </remarks>
    internal enum SecurityCredentialUse
    {
        /// <summary>
        /// SECPKG_CRED_OUTBOUND
        /// </summary>
        Outbound = 0x2
    }
}