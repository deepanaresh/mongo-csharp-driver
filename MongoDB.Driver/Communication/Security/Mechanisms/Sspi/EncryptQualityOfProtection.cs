
namespace MongoDB.Driver.Communication.Security.Mechanisms.Sspi
{
    /// <summary>
    /// Flags for EncryptMessage.
    /// </summary>
    /// <remarks>
    /// See the fQOP parameter at 
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa375378(v=vs.85).aspx.
    /// </remarks>
    internal enum EncryptQualityOfProtection : uint
    {
        /// <summary>
        /// SECQOP_WRAP_NO_ENCRYPT
        /// </summary>
        WrapNoEncrypt = 0x80000001
    }
}