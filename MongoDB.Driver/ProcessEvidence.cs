using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver
{
    /// <summary>
    /// Evidence of a MongoIdentity via the currently executing process.
    /// </summary>
    public sealed class ProcessEvidence : MongoIdentityEvidence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEvidence" /> class.
        /// </summary>
        public ProcessEvidence()
        { }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is ProcessEvidence;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return 17 * 37 + GetType().GetHashCode();
        }
    }
}