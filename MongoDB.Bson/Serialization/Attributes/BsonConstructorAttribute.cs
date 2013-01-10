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

namespace MongoDB.Bson.Serialization.Attributes
{
    /// <summary>
    /// Specifies that this constructor should be used for constructor-based deserialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class BsonConstructorAttribute : Attribute, IBsonCreatorMapAttribute
    {
        // private fields
        private string[] _memberNames;

        // constructors
        /// <summary>
        /// Initializes a new instance of the BsonConstructorAttribute class.
        /// </summary>
        public BsonConstructorAttribute()
        { }

        /// <summary>
        /// Initializes a new instance of the BsonConstructorAttribute class.
        /// </summary>
        /// <param name="memberNames">The names of the members that the constructor parameter values come from.</param>
        public BsonConstructorAttribute(params string[] memberNames)
        {
            _memberNames = memberNames;
        }

        // public properties
        /// <summary>
        /// Gets the names of the members that the constructor parameter values come from.
        /// </summary>
        public string[] MemberNames
        {
            get { return _memberNames; }
        }

        // public methods
        /// <summary>
        /// Applies a modification to the creator map.
        /// </summary>
        /// <param name="creatorMap">The creator map.</param>
        public void Apply(BsonCreatorMap creatorMap)
        {
            if (_memberNames != null)
            {
                var classMap = creatorMap.ClassMap;

                var parameters = new List<BsonMemberMap>();
                foreach (var memberName in _memberNames)
                {
                    var memberMap = classMap.GetMemberMap(memberName);
                    if (memberMap == null)
                    {
                        var message = string.Format("Class '{0}' does not have a mapped member named '{1}'.", classMap.ClassType.FullName, memberName);
                        throw new BsonSerializationException(message);
                    }
                    parameters.Add(memberMap);
                }

                creatorMap.SetParameters(parameters);
            }
        }
    }
}