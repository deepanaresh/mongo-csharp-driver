using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Internal;

namespace MongoDB.Driver.Communication.Security.Mechanisms
{
    /// <summary>
    /// A mechanism implementing the GSS API specification.
    /// </summary>
    internal class GssapiMechanism : ISaslMechanism
    {
        // public properties
        /// <summary>
        /// Gets the name of the mechanism.
        /// </summary>
        public string Name
        {
            get { return "GSSAPI"; }
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
            // TODO: provide an override to force the use of gsasl.
            bool useGsasl = !Environment.OSVersion.Platform.ToString().Contains("Win");
            if (useGsasl)
            {
                return new GsaslGssapiImplementation(
                    connection.ServerInstance.Address.Host,
                    identity);
            }

            return new WindowsGssapiImplementation(
                connection.ServerInstance.Address.Host,
                identity);
        }
    }
}