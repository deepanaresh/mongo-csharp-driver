using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MongoDB.Driver.Security.Mechanisms
{
    /// <summary>
    /// A mechanism implementing the CRAM-MD5 sasl specification.
    /// </summary>
    internal class GsaslCramMD5Mechanism : AbstractGsaslMechanism
    {
        // private fields
        private readonly MongoClientIdentity _identity;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GsaslCramMD5Mechanism" /> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        public GsaslCramMD5Mechanism(MongoClientIdentity identity)
            : base("CRAM-MD5")
        {
            _identity = identity;
        }

        // protected methods
        /// <summary>
        /// Gets the properties that should be used in the specified mechanism.
        /// </summary>
        /// <returns>The properties.</returns>
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