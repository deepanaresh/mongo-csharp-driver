using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver
{
    /// <summary>
    /// Represents an identity defined inside mongodb.
    /// </summary>
    public class MongoInternalIdentity : MongoIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoInternalIdentity" /> class.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="username">The username.</param>
        public MongoInternalIdentity(string databaseName, string username)
            : base(databaseName, username)
        { }
    }
}
