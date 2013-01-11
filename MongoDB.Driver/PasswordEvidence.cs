using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
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
        private readonly string _hash;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordEvidence" /> class.
        /// </summary>
        /// <param name="password">The password.</param>
        public PasswordEvidence(SecureString password)
        {
            _securePassword = password;
            _hash = GenerateHash(password);
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

        // public methods
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="rhs">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object rhs)
        {
            if (object.ReferenceEquals(rhs, null) || GetType() != rhs.GetType()) { return false; }

            return _hash == ((PasswordEvidence)rhs)._hash;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return 17 * 37 + _hash.GetHashCode();
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

        private static string CreateString(SecureString secureString)
        {
            IntPtr strPtr = IntPtr.Zero;
            if (secureString == null || secureString.Length == 0)
            {
                return string.Empty;
            }

            try
            {
                strPtr = Marshal.SecureStringToBSTR(secureString);
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

        /// <summary>
        /// Computes the hash value of the secured string 
        /// </summary>
        private static string GenerateHash(SecureString secureString)
        {
            IntPtr unmanagedRef = Marshal.SecureStringToBSTR(secureString);
            // stored with 0's in between each character...
            byte[] bytes = new byte[secureString.Length * 2];
            var byteArrayHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            Marshal.Copy(unmanagedRef, bytes, 0, secureString.Length * 2);
            using (var SHA256 = new SHA256Managed())
            {
                try
                {
                    return Convert.ToBase64String(SHA256.ComputeHash(bytes));
                }
                finally
                {
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = (byte)'\0';
                    }
                    byteArrayHandle.Free();
                    Marshal.ZeroFreeBSTR(unmanagedRef);
                }
            }
        }
    }
}
