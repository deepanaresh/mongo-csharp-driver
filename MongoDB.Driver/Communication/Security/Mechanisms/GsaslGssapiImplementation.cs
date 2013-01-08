using System.Collections.Generic;

namespace MongoDB.Driver.Communication.Security.Mechanisms
{
    /// <summary>
    /// Implements the GssApi specification using the Gsasl library.
    /// </summary>
    internal class GsaslGssapiImplementation : AbstractGsaslImplementation
    {
        // private fields
        private readonly string _authorizationId;
        private readonly MongoClientIdentity _identity;
        private readonly string _servicePrincipalName;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GsaslGssapiImplementation" /> class.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="identity">The identity.</param>
        public GsaslGssapiImplementation(string serverName, MongoClientIdentity identity)
            : base("GSSAPI", new byte[0])
        {
            _authorizationId = identity.Username;
            _servicePrincipalName = "mongodb/" + serverName;
            _identity = identity;
        }

        // protected methods
        /// <summary>
        /// Gets the properties that should be used in the specified mechanism.
        /// </summary>
        /// <returns>The properties.</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetProperties()
        {
            yield return new KeyValuePair<string, string>("AUTHZID", _authorizationId);
            yield return new KeyValuePair<string, string>("AUTHID", _identity.Username);
            yield return new KeyValuePair<string, string>("PASSWORD", _identity.Password); // TODO: fix this to be secure
            var atIndex = _identity.Username.LastIndexOf("@");
            if (atIndex != -1 && atIndex != _identity.Username.Length - 1)
            {
                var realm = _identity.Username.Substring(atIndex + 1);
                yield return new KeyValuePair<string, string>("REALM", realm);
            }
            yield return new KeyValuePair<string, string>("SERVICE", _servicePrincipalName);
        }
    }
}