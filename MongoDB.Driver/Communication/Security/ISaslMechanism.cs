using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Internal;

namespace MongoDB.Driver.Communication.Security
{
    /// <summary>
    /// Represents a sasl mechanism.
    /// </summary>
    internal interface ISaslMechanism
    {
        /// <summary>
        /// Gets the name of the mechanism.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Determines whether this instance can authenticate with the specified credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>
        ///   <c>true</c> if this instance can authenticate with the specified credentials; otherwise, <c>false</c>.
        /// </returns>
        bool CanUse(MongoCredentials credentials);

        /// <summary>
        /// Initializes the mechanism.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The initial step.</returns>
        ISaslStep Initialize(MongoConnection connection, MongoCredentials credentials);
    }
}