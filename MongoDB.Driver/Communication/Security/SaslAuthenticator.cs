using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Internal;
using MongoDB.Driver.Communication.Security;
using MongoDB.Driver.Communication.Security.Mechanisms;

namespace MongoDB.Driver.Communication.Security
{
    /// <summary>
    /// Authenticates credentials using the SASL protocol.
    /// </summary>
    internal class SaslAuthenticator
    {
        // public methods
        /// <summary>
        /// Authenticates the connection against the given database.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="mechanism">The mechanism.</param>
        public void Authenticate(MongoConnection connection, MongoCredentials credentials, ISaslMechanism mechanism)
        {
            using (var conversation = new SaslConversation())
            {
                var currentStep = mechanism.Initialize(connection, credentials);

                var command = new CommandDocument
                {
                    { "saslStart", 1 },
                    { "mechanism", mechanism.Name },
                    { "payload", currentStep.BytesToSendToServer }
                };

                while (true)
                {
                    var result = connection.RunCommand(credentials.Source, QueryFlags.SlaveOk, command, true);
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

        private void HandleError(CommandResult result, int code)
        {
            throw new MongoSecurityException(string.Format("Error: {0} - {1}", code, result.Response["errmsg"].AsString));
        }
    }
}