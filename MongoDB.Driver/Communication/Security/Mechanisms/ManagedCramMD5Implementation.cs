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
        private readonly string _username;
        private readonly string _password;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedCramMD5Implementation" /> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public ManagedCramMD5Implementation(string username, string password)
        {
            _username = username;
            _password = password;
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
            var mongoPassword = _username + ":mongo:" + _password;
            byte[] password;
            using (var md5 = MD5.Create())
            {
                password = GetMongoPassword(md5, encoding, _username, _password);
                var temp = ToHexString(password);
                password = encoding.GetBytes(temp);
            }

            byte[] digest;
            using (var hmacMd5 = new HMACMD5(password))
            {
                digest = hmacMd5.ComputeHash(bytesReceivedFromServer);
            }

            var response = _username + " " + ToHexString(digest);
            var bytesToSendToServer = encoding.GetBytes(response);

            return new SaslCompletionStep(bytesToSendToServer);
        }
    }
}
