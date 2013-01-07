using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver.Internal;

namespace MongoDB.Driver.Communication.Security.Mechanisms
{
    /// <summary>
    /// A mechanism implementing the CRAM-MD5 sasl specification.
    /// </summary>
    internal class GsaslCramMD5Mechanism : ISaslMechanism
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
            return new GsaslCramMD5Step(identity);
        }

        /// <summary>
        /// A mechanism implementing the CRAM-MD5 sasl specification.
        /// </summary>
        private class GsaslCramMD5Step : AbstractGsaslStep
        {
            // private fields
            private readonly MongoClientIdentity _identity;

            // constructors
            public GsaslCramMD5Step(MongoClientIdentity identity)
                : base("CRAM-MD5", new byte[0])
            {
                _identity = identity;
            }

            // protected methods
            protected override IEnumerable<KeyValuePair<string, string>> GetProperties()
            {
                yield return new KeyValuePair<string, string>("AUTHID", _identity.Username);
                yield return new KeyValuePair<string, string>("PASSWORD", CreatePassword());
            }

            // private methods
            private string CreatePassword()
            {
                var mongoPassword = _identity.Username + ":mongo:" + _identity.Password;
                byte[] password;
                using (var md5 = MD5.Create())
                {
                    password = md5.ComputeHash(Encoding.UTF8.GetBytes(mongoPassword));
                }

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < password.Length; i++)
                {
                    builder.Append(password[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
   }
}