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
using System.Linq;
using System.Reflection;

namespace MongoDB.Bson.Serialization
{
    /// <summary>
    /// Represents a mapping to a delegate and its parameters.
    /// </summary>
    public class BsonCreatorMap
    {
        // private fields
        private readonly BsonClassMap _classMap;
        private readonly MemberInfo _memberInfo;
        private readonly Delegate _delegate;
        private IEnumerable<BsonMemberMap> _parameters;

        // constructors
        /// <summary>
        /// Initializes a new instance of the BsonCreatorMap class.
        /// </summary>
        /// <param name="classMap">The class map.</param>
        /// <param name="memberInfo">The member info (null if none).</param>
        /// <param name="delegate">The delegate.</param>
        public BsonCreatorMap(BsonClassMap classMap, MemberInfo memberInfo, Delegate @delegate)
        {
            if (classMap == null)
            {
                throw new ArgumentNullException("classMap");
            }
            if (@delegate == null)
            {
                throw new ArgumentNullException("delegate");
            }

            _classMap = classMap;
            _memberInfo = memberInfo;
            _delegate = @delegate;
        }

        // public properties
        /// <summary>
        /// Gets the class map that this creator map belongs to.
        /// </summary>
        public BsonClassMap ClassMap
        {
            get { return _classMap; }
        }

        /// <summary>
        /// Gets the delegeate
        /// </summary>
        public Delegate Delegate
        {
            get { return _delegate; }
        }

        public MemberInfo MemberInfo
        {
            get { return _memberInfo; }
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
        /// Sets the parameters for the creator map.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The creator map.</returns>
        public BsonCreatorMap SetParameters(IEnumerable<BsonMemberMap> parameters)
        {
            _parameters = parameters.ToArray(); // force execution of any LINQ queries
            return this;
        }
    }
}
