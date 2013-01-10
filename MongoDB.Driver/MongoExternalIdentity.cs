using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver
{
    /// <summary>
    /// Represents an identity defined outside of mongodb.
    /// </summary>
    public class MongoExternalIdentity : MongoIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoExternalIdentity" /> class.
        /// </summary>
        /// <param name="username">The username.</param>
        public MongoExternalIdentity(string username)
            : base("$external", username)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoExternalIdentity" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="username">The username.</param>
        public MongoExternalIdentity(string source, string username)
            : base(source, username)
        { }
    }
}
