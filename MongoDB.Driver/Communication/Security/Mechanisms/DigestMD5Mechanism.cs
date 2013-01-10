using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MongoDB.Driver.Communication.Security.Mechanisms
{
    /// <summary>
    /// A mechanism for DIGEST-MD5.
    /// </summary>
    internal class DigestMD5Mechanism : ISaslMechanism
    {
        // public properties
        /// <summary>
        /// Gets the name of the mechanism.
        /// </summary>
        public string Name
        {
            get { return "DIGEST-MD5"; }
        }

        // public methods
        /// <summary>
        /// Determines whether this instance can authenticate with the specified credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>
        ///   <c>true</c> if this instance can authenticate with the specified credentials; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool CanUse(MongoCredentials credentials)
        {
            return credentials.AuthenticationType == MongoAuthenticationType.Negotiate &&
                credentials.Evidence is PasswordEvidence;
        }

        /// <summary>
        /// Initializes the mechanism.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The initial step.</returns>
        public ISaslStep Initialize(Internal.MongoConnection connection, MongoCredentials credentials)
        {
            return new ManagedDigestMD5Implementation(
                connection.ServerInstance.Address.Host,
                credentials.Username,
                ((PasswordEvidence)credentials.Evidence).Password);
        }       
    }
}