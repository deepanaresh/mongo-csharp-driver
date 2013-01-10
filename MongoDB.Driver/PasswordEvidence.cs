using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace MongoDB.Driver
{
    /// <summary>
    /// Evidence of a MongoIdentity via a shared secret.
    /// </summary>
    public sealed class PasswordEvidence : MongoIdentityEvidence
    {
        // private fields
        private readonly SecureString _securePassword;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordEvidence" /> class.
        /// </summary>
        /// <param name="password">The password.</param>
        public PasswordEvidence(SecureString password)
        {
            _securePassword = password;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordEvidence" /> class.
        /// </summary>
        /// <param name="password">The password.</param>
        public PasswordEvidence(string password)
            : this(CreateSecureString(password))
        { }

        // public properties
        /// <summary>
        /// Gets the password.
        /// </summary>
        public string Password
        {
            get { return CreateString(_securePassword); }
        }

        /// <summary>
        /// Gets the secure password.
        /// </summary>
        public SecureString SecurePassword
        {
            get { return _securePassword; }
        }

        // private static methods
        private static SecureString CreateSecureString(string str)
        {
            if (str != null)
            {
                var secureStr = new SecureString();
                foreach (var c in str)
                {
                    secureStr.AppendChar(c);
                }
                return secureStr;
            }

            return null;
        }

        private static string CreateString(SecureString secureStr)
        {
            IntPtr strPtr = IntPtr.Zero;
            if (secureStr == null || secureStr.Length == 0)
            {
                return string.Empty;
            }

            try
            {
                strPtr = Marshal.SecureStringToBSTR(secureStr);
                return Marshal.PtrToStringBSTR(strPtr);
            }
            finally
            {
                if (strPtr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(strPtr);
                }
            }
        }
    }
}
