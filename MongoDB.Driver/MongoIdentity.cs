﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;

namespace MongoDB.Driver
{
    /// <summary>
    /// Represents an identity in MongoDB.
    /// </summary>
    public abstract class MongoIdentity : IEquatable<MongoIdentity>
    {
        // private fields
        private readonly string _source;
        private readonly string _username;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoIdentity" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="username">The username.</param>
        internal MongoIdentity(string source, string username)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }

            _source = source;
            _username = username;
        }

        // public properties
        /// <summary>
        /// Gets the source.
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

        // public operators
        /// <summary>
        /// Compares two MongoCredentials.
        /// </summary>
        /// <param name="lhs">The first MongoCredentials.</param>
        /// <param name="rhs">The other MongoCredentials.</param>
        /// <returns>True if the two MongoCredentials are equal (or both null).</returns>
        public static bool operator ==(MongoIdentity lhs, MongoIdentity rhs)
        {
            return object.Equals(lhs, rhs);
        }

        /// <summary>
        /// Compares two MongoCredentials.
        /// </summary>
        /// <param name="lhs">The first MongoCredentials.</param>
        /// <param name="rhs">The other MongoCredentials.</param>
        /// <returns>True if the two MongoCredentials are not equal (or one is null and the other is not).</returns>
        public static bool operator !=(MongoIdentity lhs, MongoIdentity rhs)
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
        public override bool Equals(object obj)
        {
            return Equals(obj as MongoIdentity);
        }

        /// <summary>
        /// Determines whether the specified instance is equal to this instance.
        /// </summary>
        /// <param name="rhs">The right-hand side.</param>
        /// <returns>
        ///   <c>true</c> if the specified instance is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(MongoIdentity rhs)
        {
            if (object.ReferenceEquals(rhs, null) || GetType() != rhs.GetType()) { return false; }

            return _username == rhs._username && _source == rhs._source;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hash = 17;
            hash += 37 * _username.GetHashCode();
            hash += 37 * _source.GetHashCode();
            return hash;
        }
    }
}