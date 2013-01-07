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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MongoDB.Bson.Serialization
{
    /// <summary>
    /// Represents the mapping between a constructor and a set of BSON elements.
    /// </summary>
    public class BsonConstructorMap
    {
        // private fields
        private BsonClassMap _classMap;
        private readonly ConstructorInfo _constructorInfo;
        private IEnumerable<BsonMemberMap> _parameters;

        // constructors
        /// <summary>
        /// Initializes a new instance of the BsonConstructorMap class.
        /// </summary>
        /// <param name="classMap">The class map.</param>
        /// <param name="constructorInfo">The constructor info.</param>
        public BsonConstructorMap(BsonClassMap classMap, ConstructorInfo constructorInfo)
        {
            _classMap = classMap;
            _constructorInfo = constructorInfo;
        }

        // public properties
        /// <summary>
        /// Gets the class map that this member map belongs to.
        /// </summary>
        public BsonClassMap ClassMap
        {
            get { return _classMap; }
        }

        /// <summary>
        /// Gets the constructor info.
        /// </summary>
        public ConstructorInfo ConstructorInfo
        {
            get { return _constructorInfo; }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public IEnumerable<BsonMemberMap> Parameters
        {
            get { return _parameters; }
        }

        // public methods
        /// <summary>
        /// Sets the parameters for the constructor map.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The constructor map.</returns>
        public BsonConstructorMap SetParameters(IEnumerable<BsonMemberMap> parameters)
        {
            _parameters = parameters.ToArray(); // force execution of any LINQ queries
            return this;
        }
    }
}
