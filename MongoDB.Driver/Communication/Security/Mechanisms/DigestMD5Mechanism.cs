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
        /// Initializes the mechanism.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>The initial step.</returns>
        public ISaslStep Initialize(Internal.MongoConnection connection, MongoClientIdentity identity)
        {
            return new ManagedDigestMD5Implementation(connection.ServerInstance.Address.Host, identity);
        }       
    }
}