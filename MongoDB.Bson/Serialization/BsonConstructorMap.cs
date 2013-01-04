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
using System.Reflection;

namespace MongoDB.Bson.Serialization
{
    /// <summary>
    /// Represents the mapping between a constructor and a set of BSON elements.
    /// </summary>
    public class BsonConstructorMap
    {
        // private fields
        private readonly ConstructorInfo _constructorInfo;
        private readonly IEnumerable<BsonMemberMap> _parameters;

        // constructors
        /// <summary>
        /// Initializes a new instance of the BsonConstructorMap class.
        /// </summary>
        /// <param name="constructorInfo">The constructor info.</param>
        /// <param name="parameters">The parameters.</param>
        public BsonConstructorMap(ConstructorInfo constructorInfo, IEnumerable<BsonMemberMap> parameters)
        {
            _constructorInfo = constructorInfo;
            _parameters = parameters;
        }

        // public properties
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
    }
}
