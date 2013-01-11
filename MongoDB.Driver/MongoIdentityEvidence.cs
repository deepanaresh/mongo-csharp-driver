﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver
{
    /// <summary>
    /// Evidence used as proof of a MongoIdentity.
    /// </summary>
    public abstract class MongoIdentityEvidence
    {
        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoIdentityEvidence" /> class.
        /// </summary>
        internal MongoIdentityEvidence()
        { }

        // public operators
        /// <summary>
        /// Compares two MongoCredentials.
        /// </summary>
        /// <param name="lhs">The first MongoCredentials.</param>
        /// <param name="rhs">The other MongoCredentials.</param>
        /// <returns>True if the two MongoCredentials are equal (or both null).</returns>
        public static bool operator ==(MongoIdentityEvidence lhs, MongoIdentityEvidence rhs)
        {
            return object.Equals(lhs, rhs);
        }

        /// <summary>
        /// Compares two MongoCredentials.
        /// </summary>
        /// <param name="lhs">The first MongoCredentials.</param>
        /// <param name="rhs">The other MongoCredentials.</param>
        /// <returns>True if the two MongoCredentials are not equal (or one is null and the other is not).</returns>
        public static bool operator !=(MongoIdentityEvidence lhs, MongoIdentityEvidence rhs)
        {
            return !(lhs == rhs);
        }

        // public methods
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotImplementedException">Subclasses of MongoIdentityEvidence must override Equals.</exception>
        public override bool Equals(object obj)
        {
            throw new NotImplementedException("Subclasses of MongoIdentityEvidence must override Equals.");
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <exception cref="System.NotImplementedException">Subclasses of MongoIdentityEvidence must override Equals.</exception>
        public override int GetHashCode()
        {
            throw new NotImplementedException("Subclasses of MongoIdentityEvidence must override GetHashCode.");
        }
    }
}
