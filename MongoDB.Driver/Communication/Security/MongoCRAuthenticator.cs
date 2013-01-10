using System;
using System.Collections.Generic;
using MongoDB.Driver.Internal;

namespace MongoDB.Driver.Communication.Security
{
    /// <summary>
    /// Authenticates credentials using the MONGO-CR protocol.
    /// </summary>
    internal class MongoCRAuthenticator
    {
        // public methods
        /// <summary>
        /// Authenticates the connection against the given database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="credentials">The credentials.</param>
        public void Authenticate(MongoConnection connection, MongoCredentials credentials)
        {
            var nonceCommand = new CommandDocument("getnonce", 1);
            var commandResult = connection.RunCommand(credentials.Source, QueryFlags.None, nonceCommand, false);
            if (!commandResult.Ok)
            {
                throw new MongoAuthenticationException(
                    "Error getting nonce for authentication.",
                    new MongoCommandException(commandResult));
            }

            var nonce = commandResult.Response["nonce"].AsString;
            var passwordDigest = MongoUtils.Hash(credentials.Username + ":mongo:" + ((PasswordEvidence)credentials.Evidence).Password);
            var digest = MongoUtils.Hash(nonce + credentials.Username + passwordDigest);
            var authenticateCommand = new CommandDocument
                {
                    { "authenticate", 1 },
                    { "user", credentials.Username },
                    { "nonce", nonce },
                    { "key", digest }
                };

            commandResult = connection.RunCommand(credentials.Source, QueryFlags.None, authenticateCommand, false);
            if (!commandResult.Ok)
            {
                var message = string.Format("Invalid credentials for database '{0}'.", credentials.Source);
                throw new MongoAuthenticationException(
                    message,
                    new MongoCommandException(commandResult));
            }
        }
    }
}
