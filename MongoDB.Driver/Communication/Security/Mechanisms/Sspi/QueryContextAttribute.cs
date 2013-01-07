﻿
namespace MongoDB.Driver.Communication.Security.Mechanisms.Sspi
{
    /// <summary>
    /// Flags for QueryContextAttributes.
    /// </summary>
    /// <remarks>
    /// See the ulAttribute parameter at 
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa379326(v=vs.85).aspx.
    /// </remarks>
    internal enum QueryContextAttributes
    {
        /// <summary>
        /// SECPKG_ATTR_SIZES
        /// </summary>
        Sizes = 0x0 
    }
}
