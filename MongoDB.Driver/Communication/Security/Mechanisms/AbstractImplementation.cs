using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MongoDB.Driver.Communication.Security.Mechanisms
{
    /// <summary>
    /// Base implementation for a sasl step to provide some common methods.
    /// </summary>
    internal abstract class AbstractImplementation
    {
        // protected methods
        /// <summary>
        /// Gets the mongo password.
        /// </summary>
        /// <param name="md5">The MD5.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        protected byte[] GetMongoPassword(MD5 md5, Encoding encoding, string username, string password)
        {
            var mongoPassword = username + ":mongo:" + password;
            return md5.ComputeHash(encoding.GetBytes(mongoPassword));
        }

        /// <summary>
        /// To the hex string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        protected string ToHexString(byte[] bytes)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}