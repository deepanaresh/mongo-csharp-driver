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
        private static readonly List<ISaslMechanism> __negotiatedMechanisms;

        // private fields
        private readonly MongoConnection _connection;
        private readonly MongoCredentials _credentials;
        private bool _isAuthenticated;

        // constructors
        /// <summary>
        /// Initializes the <see cref="SaslAuthenticationStore" /> class.
        /// </summary>
        static SaslAuthenticationStore()
        {
            __negotiatedMechanisms = new List<ISaslMechanism>
            {
                new GssapiMechanism(),
                new CramMD5Mechanism(),
                new DigestMD5Mechanism()
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaslAuthenticationStore" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="identity">The identity.</param>
        public SaslAuthenticationStore(MongoConnection connection, MongoCredentials credentials)
        {
            _connection = connection;
            _credentials = credentials;
            _isAuthenticated = credentials == null;
        }

        // public methods
        /// <summary>
        /// Authenticates the connection against the given database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="credentials">The credentials.</param>
        public void Authenticate(string databaseName, MongoCredentials credentials)
        {
            if (!_isAuthenticated)
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
                var mechanism = GetMechanism(_connection);
                var currentStep = mechanism.Initialize(_connection, _credentials);

                var command = new CommandDocument
                {
                    { "saslStart", 1 },
                    { "mechanism", mechanism.Name },
                    { "payload", currentStep.BytesToSendToServer }
                };

                while (true)
                {
                    var result = _connection.RunCommand(_credentials.Source, QueryFlags.SlaveOk, command, true);
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

        private ISaslMechanism GetMechanism(MongoConnection connection)
        {
            var command = new CommandDocument
            {
                { "saslStart", 1 },
                { "mechanism", ""}, // forces a response that contains a list of supported mechanisms...
                { "payload", new byte[0] }
            };

            var result = _connection.RunCommand(_credentials.Source, QueryFlags.SlaveOk, command, true);
            if (result.Response.Contains("supportedMechanisms"))
            {
                var serverMechanisms = result.Response["supportedMechanisms"].AsBsonArray.ToLookup(x => x.AsString);
                foreach (var mechanism in __negotiatedMechanisms)
                {
                    if (serverMechanisms.Contains(mechanism.Name) && mechanism.CanUse(_credentials))
                    {
                        return mechanism;
                    }
                }
            }

            throw new MongoSecurityException("Unable to negotiate a security protocol with the server.");
        }

        private void HandleError(CommandResult result, int code)
        {
            throw new MongoSecurityException(string.Format("Error: {0} - {1}", code, result.Response["errmsg"].AsString));
        }
    }
}