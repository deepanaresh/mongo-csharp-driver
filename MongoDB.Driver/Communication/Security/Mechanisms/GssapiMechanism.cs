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
        /// Determines whether this instance can authenticate with the specified credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>
        ///   <c>true</c> if this instance can authenticate with the specified credentials; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool CanUse(MongoCredentials credentials)
        {
            return credentials.Protocol == MongoAuthenticationProtocol.Gssapi;
        }

        /// <summary>
        /// Initializes the mechanism.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The initial step.</returns>
        public ISaslStep Initialize(MongoConnection connection, MongoCredentials credentials)
        {
            // TODO: provide an override to force the use of gsasl.
            bool useGsasl = !Environment.OSVersion.Platform.ToString().Contains("Win");
            if (useGsasl)
            {
                throw new NotImplementedException("Gssapi Support on Non-Windows Machinse is Not Implemented.");
                //return new GsaslGssapiImplementation(
                //    connection.ServerInstance.Address.Host,
                //    credentials.Username,
                //    credentials.Evidence);
            }

            return new WindowsGssapiImplementation(
                connection.ServerInstance.Address.Host,
                credentials.Username,
                credentials.Evidence);
        }
    }
}