﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver.Internal;

namespace MongoDB.Driver.Communication.Security.Mechanisms
{
    /// <summary>
    /// A mechanism for CRAM-MD5.
    /// </summary>
    internal class CramMD5Mechanism : ISaslMechanism
    {
        // public properties
        /// <summary>
        /// Gets the name of the mechanism.
        /// </summary>
        public string Name
        {
            get { return "CRAM-MD5"; }
        }

        // public methods
        /// <summary>
        /// Initializes the mechanism.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>The initial step.</returns>
        public ISaslStep Initialize(MongoConnection connection, MongoClientIdentity identity)
        {
            return new ManagedCramMD5Implementation(identity);
            //return new GsaslCramMD5Implementation(identity);
        }
    }
}