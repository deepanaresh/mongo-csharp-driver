using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MongoDB.Driver.Communication.Security.Mechanisms
{
    /// <summary>
    /// Managed implementation of the CRAM-MD5 sasl spec (http://tools.ietf.org/html/draft-ietf-sasl-crammd5-10).
    /// </summary>
    internal class ManagedCramMD5Implementation : AbstractImplementation, ISaslStep
    {
        // private fields
        private readonly MongoClientIdentity _identity;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedCramMD5Implementation" /> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        public ManagedCramMD5Implementation(MongoClientIdentity identity)
        {
            _identity = identity;
        }

        // public methods
        /// <summary>
        /// The bytes that should be sent to ther server before calling Transition.
        /// </summary>
        public byte[] BytesToSendToServer
        {
            get { return new byte[0]; }
        }

        // public methods
        public ISaslStep Transition(SaslConversation conversation, byte[] bytesReceivedFromServer)
        {
            var encoding = Encoding.UTF8;
            var mongoPassword = _identity.Username + ":mongo:" + _identity.Password;
            byte[] password;
            using (var md5 = MD5.Create())
            {
                password = GetMongoPassword(md5, encoding, _identity.Username, _identity.Password);
                var temp = ToHexString(password);
                password = encoding.GetBytes(temp);
            }

            byte[] digest;
            using (var hmacMd5 = new HMACMD5(password))
            {
                digest = hmacMd5.ComputeHash(bytesReceivedFromServer);
            }

            var response = _identity.Username + " " + ToHexString(digest);
            var bytesToSendToServer = encoding.GetBytes(response);

            return new SaslCompletionStep(bytesToSendToServer);
        }
    }
}
