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

namespace MongoDB.Bson.Serialization.Conventions
{
    /// <summary>
    /// A convention that uses the names of the constructor parameters to find the matching members.
    /// </summary>
    public class NamedParameterConstructorMapConvention : ConventionBase, IConstructorMapConvention
    {
        // public methods
        /// <summary>
        /// Applies a modification to the constructor map.
        /// </summary>
        /// <param name="constructorMap">The constructor map.</param>
        public void Apply(BsonConstructorMap constructorMap)
        {
            if (constructorMap.Parameters == null)
            {
                var memberMaps = new List<BsonMemberMap>();

                var classMap = constructorMap.ClassMap;
                var parameterNames = constructorMap.ConstructorInfo.GetParameters().Select(p => p.Name);
                foreach (var parameterName in parameterNames)
                {
                    // TODO: should really be using AllMemberMaps but that is not populated until Freeze is called
                    var matchingMemberMaps = classMap.DeclaredMemberMaps.Where(m => string.Compare(m.MemberName, parameterName, true) == 0).ToArray(); // ignoreCase
                    if (matchingMemberMaps.Length == 1)
                    {
                        memberMaps.Add(matchingMemberMaps[0]);
                    }
                    else if (matchingMemberMaps.Length > 1)
                    {
                        var message = string.Format("Constructor parameter '{0}' of class '{1}' does not match any member.", parameterName, classMap.ClassType.FullName);
                        throw new BsonSerializationException(message);
                    }
                    else
                    {
                        var message = string.Format("Constructor parameter '{0}' of class '{1}' matches multiple members.", parameterName, classMap.ClassType.FullName);
                        throw new BsonSerializationException(message);
                    }
                }

                constructorMap.SetParameters(memberMaps);
            }
        }
    }
}