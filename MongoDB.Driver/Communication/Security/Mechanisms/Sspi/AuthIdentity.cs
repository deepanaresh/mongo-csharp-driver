using System;
using System.Runtime.InteropServices;
using System.Security;

namespace MongoDB.Driver.Communication.Security.Mechanisms.Sspi
{
    /// <summary>
    /// SEC_WINNT_AUTH_IDENTITY
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class AuthIdentity : IDisposable
    {
        // public fields
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Username;
        public int UsernameLength;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Domain;
        public int DomainLength;
        public IntPtr Password;
        public int PasswordLength;
        public AuthIdentityFlag Flags;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthIdentity" /> struct.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public AuthIdentity(string username, SecureString password)
        {
            Username = null;
            UsernameLength = 0;
            if (!string.IsNullOrEmpty(username))
            {
                Username = username;
                UsernameLength = username.Length;
            }

            Password = IntPtr.Zero;
            PasswordLength = 0;
            
            if (password != null && password.Length > 0)
            {
                Password = Marshal.SecureStringToGlobalAllocUnicode(password);
                PasswordLength = password.Length;
            }

            Domain = null;
            DomainLength = 0;

            Flags = AuthIdentityFlag.Unicode;
        }

        ~AuthIdentity()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            if (Password != IntPtr.Zero)
            {
                Marshal.ZeroFreeGlobalAllocUnicode(Password);
                Password = IntPtr.Zero;
            }
        }
    }
}