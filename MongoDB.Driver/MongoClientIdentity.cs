using System;
using System.Runtime.InteropServices;
using System.Security;

namespace MongoDB.Driver
{
    /// <summary>
    /// Represents the identity to be used when talking with mongodb.
    /// </summary>
    public abstract class MongoClientIdentity
    {
        // private static fields
        private readonly static MongoClientIdentity _system = SystemMongoClientIdentity.Instance;

        // private fields
        private readonly MongoAuthenticationType _authenticationType;
        private readonly SecureString _password;
        private readonly string _source;
        private readonly string _username;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoClientIdentity" /> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="source">The source.</param>
        /// <param name="authenticationType">Type of the authentication.</param>
        internal MongoClientIdentity(string username, SecureString password, string source, MongoAuthenticationType authenticationType)
        {
            _username = username;
            if (password != null)
            {
                _password = password.Copy();
            }
            _source = source;
            _authenticationType = authenticationType;
        }

        // public static properties
        /// <summary>
        /// Gets the system identity used to execute the current process.
        /// </summary>
        public static MongoClientIdentity System
        {
            get { return _system; }
        }

        // public static methods
        /// <summary>
        /// Creates a Gssapi MongoClientIdentity.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>An identity for authenticating with Gssapi.</returns>
        public static MongoClientIdentity Gssapi(string username, string password)
        {
            return new GssapiMongoClientIdentity(username, CreateSecureString(password));
        }

        /// <summary>
        /// Creates a Gssapi MongoClientIdentity.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>An identity for authenticating with Gssapi.</returns>
        public static MongoClientIdentity Gssapi(string username, SecureString password)
        {
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            return new GssapiMongoClientIdentity(username, password);
        }

        /// <summary>
        /// Creates a Negotiated MongoClientIdentity.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="source">The source.</param>
        /// <returns>An identity for authenticating by negotiating.</returns>
        public static MongoClientIdentity Negotiate(string username, string password, string source)
        {
            return Negotiate(username, CreateSecureString(password), source);
        }

        /// <summary>
        /// Creates a Negotiated MongoClientIdentity.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="source">The source.</param>
        /// <returns>An identity for authenticating by negotiating.</returns>
        public static MongoClientIdentity Negotiate(string username, SecureString password, string source)
        {
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return new NegotiatedMongoClientIdentity(username, password, source);
        }

        // public properties
        /// <summary>
        /// Gets the type of authentication used to confirm this identity.
        /// </summary>
        public MongoAuthenticationType AuthenticationType
        {
            get { return _authenticationType; }
        }

        /// <summary>
        /// Indicates whether this instance has a password.
        /// </summary>
        public bool HasPassword
        {
            get { return _password != null; }
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        public string Password
        {
            get 
            {
                if (HasPassword)
                {
                    return CreateString(_password);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the secure password.
        /// </summary>
        public SecureString SecurePassword
        {
            get 
            {
                if (HasPassword)
                {
                    return _password.Copy();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the source used to verify the credentials.
        /// </summary>
        public string Source
        {
            get { return _source; }
        }

        /// <summary>
        /// Gets the username.
        /// </summary>
        public string Username
        {
            get { return _username; }
        }

        // private static methods
        private static SecureString CreateSecureString(string str)
        {
            if (str != null)
            {
                var secureStr = new SecureString();
                foreach (var c in str)
                {
                    secureStr.AppendChar(c);
                }
                return secureStr;
            }

            return null;
        }

        private static string CreateString(SecureString secureStr)
        {
            IntPtr strPtr = IntPtr.Zero;
            if (secureStr == null || secureStr.Length == 0)
            {
                return string.Empty;
            }

            try
            {
                strPtr = Marshal.SecureStringToBSTR(secureStr);
                return Marshal.PtrToStringBSTR(strPtr);
            }
            finally
            {
                if (strPtr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(strPtr);
                }
            }
        }

        // nested classes
        private class GssapiMongoClientIdentity : MongoClientIdentity
        {
            public GssapiMongoClientIdentity(string username, SecureString password)
                : base(username, password, "$external", MongoAuthenticationType.Gssapi)
            { }
        }

        private class NegotiatedMongoClientIdentity : MongoClientIdentity
        {
            public NegotiatedMongoClientIdentity(string username, SecureString password, string source)
                : base(username, password, source, MongoAuthenticationType.Negotiate)
            { }
        }

        private class SystemMongoClientIdentity : GssapiMongoClientIdentity
        {
            public readonly static SystemMongoClientIdentity Instance = new SystemMongoClientIdentity();

            private SystemMongoClientIdentity()
                : base(null, (SecureString)null)
            { }
        }
    }
}