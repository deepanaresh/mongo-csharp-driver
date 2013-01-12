﻿/* Copyright 2010-2013 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Communication.Security.Mechanisms;
using MongoDB.Driver.Internal;

namespace MongoDB.Driver.Communication.Security
{
    /// <summary>
    /// Authenticates credentials against MongoDB.
    /// </summary>
    internal class Authenticator
    {
        private static readonly List<ISaslMechanism> __clientSupportedMechanisms = new List<ISaslMechanism>
        {
            new GssapiMechanism(),
            new CramMD5Mechanism(),
            new DigestMD5Mechanism()
        };

        // private fields
        private readonly IEnumerable<MongoCredentials> _credentials;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Authenticator" /> class.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        public Authenticator(IEnumerable<MongoCredentials> credentials)
        {
            _credentials = credentials;
        }

        // public methods
        /// <summary>
        /// Authenticates the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void Authenticate(MongoConnection connection)
        {
            if (!_credentials.Any())
            {
                return;
            }

            var serverSupportedMechanisms = GetServerSupportedMechanisms(connection);
            foreach (var credential in _credentials)
            {
                Authenticate(connection, credential, serverSupportedMechanisms);
            }
        }

        // private methods
        private void Authenticate(MongoConnection connection, MongoCredentials credentials, List<string> serverSupportedMechanisms)
        {
            foreach (var mechanism in __clientSupportedMechanisms)
            {
                if (serverSupportedMechanisms.Contains(mechanism.Name) && mechanism.CanUse(credentials))
                {
                    new SaslAuthenticator().Authenticate(connection, credentials, mechanism);
                    return;
                }
            }

            // We didn't find a sasl mechanism we both supported, so last resort is MONGO-CR.
            if (credentials.Protocol == MongoAuthenticationProtocol.Strongest && credentials.Evidence is PasswordEvidence)
            {
                new MongoCRAuthenticator().Authenticate(connection, credentials);
                return;
            }

            var message = string.Format("Unable to negotiate a protocol to authenticate. Credentials for source {0}, username {1} over protocol {2} could not be authenticated", credentials.Source, credentials.Username, credentials.Protocol);
            throw new MongoSecurityException(message);
        }

        private List<string> GetServerSupportedMechanisms(MongoConnection connection)
        {
            var command = new CommandDocument
            {
                { "saslStart", 1 },
                { "mechanism", ""}, // forces a response that contains a list of supported mechanisms...
                { "payload", new byte[0] }
            };

            var result = connection.RunCommand("admin", QueryFlags.SlaveOk, command, false);
            if (result.Response.Contains("supportedMechanisms"))
            {
                return result.Response["supportedMechanisms"].AsBsonArray.Select(x => x.AsString).ToList();
            }

            return new List<string>();
        }
    }

}