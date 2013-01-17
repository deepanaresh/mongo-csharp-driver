/* Copyright 2010-2013 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MongoDB.Driver
{
    /// <summary>
    /// Represents a list of credentials and the rules about how credentials can be used together.
    /// </summary>
    internal class MongoCredentialsStore : IEnumerable<MongoCredentials>
    {
        // private fields
        private readonly ReadOnlyCollection<MongoCredentials> _credentialsList;

        // constructors
        /// <summary>
        /// Creates a new instance of the MongoCredentialsStore class.
        /// </summary>
        /// <param name="credentialsList">The list of credentials.</param>
        public MongoCredentialsStore(IEnumerable<MongoCredentials> credentialsList)
        {
            var materializedList = new List<MongoCredentials>(credentialsList); // force enumeration of credentialsList
            EnsureCredentialsAreCompatibleWithEachOther(materializedList);
            _credentialsList = new ReadOnlyCollection<MongoCredentials>(materializedList);
        }

        // public methods
        /// <summary>
        /// Compares this credentials store to another credentials store.
        /// </summary>
        /// <param name="obj">The other credentials store.</param>
        /// <returns>True if the two credentials stores are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(MongoCredentialsStore))
            {
                return false;
            }

            var rhs = (MongoCredentialsStore)obj;
            return _credentialsList.SequenceEqual(rhs._credentialsList);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<MongoCredentials> GetEnumerator()
        {
            return _credentialsList.GetEnumerator();
        }

        /// <summary>
        /// Gets the hashcode for the credentials store.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            // see Effective Java by Joshua Bloch
            int hash = 17;
            foreach (var credentials in _credentialsList)
            {
                hash = 37 * hash + credentials.GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Returns a string representation of the credentials store.
        /// </summary>
        /// <returns>A string representation of the credentials store.</returns>
        public override string ToString()
        {
            return string.Format("{{{0}}}", string.Join(",", _credentialsList.Select(c => c.ToString()).ToArray()));
        }

        // explicit methods
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _credentialsList.GetEnumerator();
        }

        // private metods
        private void EnsureCredentialsAreCompatibleWithEachOther(List<MongoCredentials> list)
        {
            var sources = new HashSet<string>(list.Select(c => c.Source));
            if (sources.Count < list.Count)
            {
                throw new ArgumentException("The server currently requires that each credentials provided be from a separate source. ");
            }
        }
    }
}
