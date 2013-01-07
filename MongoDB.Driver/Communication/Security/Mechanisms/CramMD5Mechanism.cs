using System.Security.Cryptography;
using System.Text;

namespace MongoDB.Driver.Security.Mechanisms
{
    /// <summary>
    /// A mechanism implementing the CRAM-MD5 sasl specification.
    /// </summary>
    internal class CramMD5Mechanism : ISaslMechanism
    {
        // private fields
        private readonly MongoClientIdentity _identity;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CramMD5Mechanism" /> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        public CramMD5Mechanism(MongoClientIdentity identity)
        {
            _identity = identity;
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
        /// Transitions to the next step in the conversation.
        /// </summary>
        /// <param name="conversation">The conversation.</param>
        /// <param name="input">The input.</param>
        /// <returns>An ISaslStep.</returns>
        public ISaslStep Transition(SaslConversation conversation, byte[] input)
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
                digest = hmacMd5.ComputeHash(input);
            }

            var response = _identity.Username + " " + ToHexString(digest);
            var outBytes = Encoding.ASCII.GetBytes(response);

            return new SaslCompletionStep(outBytes);
        }
    }
}