using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Internal;
using MongoDB.Driver.Communication.Security;
using MongoDB.Driver.Communication.Security.Mechanisms;

namespace MongoDB.Driver.Communication.Security
{
    /// <summary>
    /// An authentication store for the SASL protocol.
    /// </summary>
    internal class SaslAuthenticationStore : IAuthenticationStore
    {
        // private static fields
        private static readonly GssapiMechanism _gssapiMechanism;
        private static readonly List<ISaslMechanism> __negotiatedMechanisms;

        // private fields
        private readonly MongoConnection _connection;
        private readonly MongoClientIdentity _identity;
        private bool _isAuthenticated;

        // constructors
        /// <summary>
        /// Initializes the <see cref="SaslAuthenticationStore" /> class.
        /// </summary>
        static SaslAuthenticationStore()
        {
            _gssapiMechanism = new GssapiMechanism();
            __negotiatedMechanisms = new List<ISaslMechanism>
            {
                new CramMD5Mechanism(),
                new DigestMD5Mechanism()
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaslAuthenticationStore" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="identity">The identity.</param>
        public SaslAuthenticationStore(MongoConnection connection, MongoClientIdentity identity)
        {
            _connection = connection;
            _identity = identity;
            _isAuthenticated = _identity == null;
        }

        // public methods
        /// <summary>
        /// Authenticates the connection against the given database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="credentials">The credentials.</param>
        public void Authenticate(string databaseName, MongoCredentials credentials)
        {
            if (_identity != null && !_isAuthenticated)
            {
                Authenticate();
                _isAuthenticated = true;
            }
        }

        /// <summary>
        /// Determines whether the connection can be authenticated against the given database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns></returns>
        public bool CanAuthenticate(string databaseName, MongoCredentials credentials)
        {
            // we can always authenticate a sasl connection...
            return true;
        }

        /// <summary>
        /// Determines whether the connection is currently authenticated against the given database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns></returns>
        public bool IsAuthenticated(string databaseName, MongoCredentials credentials)
        {
            return _isAuthenticated;
        }

        /// <summary>
        /// Logouts the connection out of the given database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        public void Logout(string databaseName)
        {
            // do nothing
        }

        // private methods
        private void Authenticate()
        {
            using (var conversation = new SaslConversation())
            {
                var mechanism = GetMechanism(_connection, _identity);
                var currentStep = mechanism.Initialize(_connection, _identity);

                var command = new CommandDocument
                {
                    { "saslStart", 1 },
                    { "mechanism", mechanism.Name },
                    { "payload", currentStep.BytesToSendToServer }
                };

                while (true)
                {
                    var result = _connection.RunCommand(_identity.Source, QueryFlags.SlaveOk, command, true);
                    var code = result.Response["code"].AsInt32;
                    if (code != 0)
                    {
                        HandleError(result, code);
                    }
                    if (result.Response["done"].AsBoolean)
                    {
                        break;
                    }

                    currentStep = currentStep.Transition(conversation, result.Response["payload"].AsByteArray);

                    command = new CommandDocument
                    {
                        { "saslContinue", 1 },
                        { "conversationId", result.Response["conversationId"].AsInt32 },
                        { "payload", currentStep.BytesToSendToServer }
                    };
                }
            }
        }

        private ISaslMechanism GetMechanism(MongoConnection connection, MongoClientIdentity identity)
        {
            switch (identity.AuthenticationType)
            {
                case MongoAuthenticationType.Gssapi:
                    return _gssapiMechanism;
                case MongoAuthenticationType.Negotiate:
                    return Negotiate(connection);
            }

            throw new NotSupportedException(string.Format("Unsupported credentials type {0}.", identity.AuthenticationType));
        }

        private void HandleError(CommandResult result, int code)
        {
            throw new MongoSecurityException(string.Format("Error: {0} - {1}", code, result.Response["errmsg"].AsString));
        }

        private ISaslMechanism Negotiate(MongoConnection connection)
        {
            var command = new CommandDocument
            {
                { "saslStart", 1 },
                { "mechanism", ""}, // forces a response that contains a list of supported mechanisms...
                { "payload", new byte[0] }
            };

            var result = _connection.RunCommand(_identity.Source, QueryFlags.SlaveOk, command, true);
            if (result.Response.Contains("supportedMechanisms"))
            {
                var supportedMechanisms = result.Response["supportedMechanisms"].AsBsonArray.ToLookup(x => x.AsString);
                foreach (var factory in __negotiatedMechanisms)
                {
                    if (supportedMechanisms.Contains(factory.Name))
                    {
                        return factory;
                    }
                }
            }

            throw new MongoSecurityException("Unable to negotiate a security protocol with the server.");
        }
    }
}