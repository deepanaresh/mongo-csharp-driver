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
using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver.Internal;

namespace MongoDB.Driver.Communication.Security.Mechanisms
{
    /// <summary>
    /// A mechanism for CRAM-MD5.
    /// </summary>
    internal class CramMD5Mechanism : ISaslMechanism
    {
        // public properties
        /// <summary>
        /// Gets the name of the mechanism.
        /// </summary>
        public string Name
        {
            get { return "CRAM-MD5"; }
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
            return credentials.Protocol == MongoAuthenticationProtocol.Strongest &&
                credentials.Evidence is PasswordEvidence;
        }

        /// <summary>
        /// Initializes the mechanism.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The initial step.</returns>
        public ISaslStep Initialize(MongoConnection connection, MongoCredentials credentials)
        {
            return new ManagedCramMD5Implementation(credentials.Username, ((PasswordEvidence)credentials.Evidence).Password);
            //return new GsaslCramMD5Implementation(identity);
        }
    }
}