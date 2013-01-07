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
        /// Initializes the mechanism.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>The initial step.</returns>
        ISaslStep Initialize(MongoConnection connection, MongoClientIdentity identity);
    }
}