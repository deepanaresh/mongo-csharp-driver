using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver.Internal;

namespace MongoDB.Driver.Communication.Security.Mechanisms
{
    /// <summary>
    /// A mechanism implementing the CRAM-MD5 sasl specification.
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
        /// Initializes the mechanism.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>The initial step.</returns>
        public ISaslStep Initialize(MongoConnection connection, MongoClientIdentity identity)
        {
            return new CramMD5Step(identity);
        }

        // nested classes
        private class CramMD5Step : ISaslStep
        {
            // private fields
            private readonly MongoClientIdentity _identity;

            // constructors
            public CramMD5Step(MongoClientIdentity identity)
            {
                _identity = identity;
            }

            // public methods
            public byte[] BytesToSendToServer
            {
                get { return new byte[0]; }
            }

            // private static methods
            private static string ToHexString(byte[] buff)
            {
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < buff.Length; i++)
                {
                    builder.Append(buff[i].ToString("x2"));
                }

                return builder.ToString();
            }

            // public methods
            public ISaslStep Transition(SaslConversation conversation, byte[] bytesReceivedFromServer)
            {
                var mongoPassword = _identity.Username + ":mongo:" + _identity.Password;
                byte[] password;
                using (var md5 = MD5.Create())
                {
                    password = md5.ComputeHash(Encoding.UTF8.GetBytes(mongoPassword));
                    var temp = ToHexString(password);
                    password = Encoding.ASCII.GetBytes(temp);
                }

                byte[] digest;
                using (var hmacMd5 = new HMACMD5(password))
                {
                    digest = hmacMd5.ComputeHash(bytesReceivedFromServer);
                }

                var response = _identity.Username + " " + ToHexString(digest);
                var bytesToSendToServer = Encoding.ASCII.GetBytes(response);

                return new SaslCompletionStep(bytesToSendToServer);
            }
        }
    }
}