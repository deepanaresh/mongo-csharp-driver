using System.Runtime.InteropServices;

namespace MongoDB.Driver.Communication.Security.Mechanisms.Sspi
{
    /// <summary>
    /// A SecPkgContext_Sizes structure.
    /// </summary>
    /// <remarks>
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa380097(v=vs.85).aspx
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SecurityPackageContextSizes
    {
        public uint MaxToken;
        public uint MaxSignature;
        public uint BlockSize;
        public uint SecurityTrailer;
    };
}
