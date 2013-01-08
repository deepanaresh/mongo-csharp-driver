using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MongoDB.Driver.Communication.Security.Mechanisms
{
    /// <summary>
    /// A mechanism implementing the CRAM-MD5 sasl specification.
    /// </summary>
    internal class GsaslCramMD5Implementation : AbstractGsaslImplementation
    {
        // private fields
        private readonly MongoClientIdentity _identity;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GsaslCramMD5Implementation" /> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        public GsaslCramMD5Implementation(MongoClientIdentity identity)
            : base("CRAM-MD5", new byte[0])
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
            using(var md5 = MD5.Create())
            {
                var bytes = GetMongoPassword(md5, Encoding.UTF8, _identity.Username, _identity.Password);
                return ToHexString(bytes);
            }
        }
    }
}