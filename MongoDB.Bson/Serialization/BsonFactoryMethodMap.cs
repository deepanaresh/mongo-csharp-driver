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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MongoDB.Bson.Serialization
{
    /// <summary>
    /// Represents a mapping to a static factory method and its parameters.
    /// </summary>
    public class BsonFactoryMethodMap : BsonCreatorMap
    {
        // private fields
        private readonly MethodInfo _methodInfo;

        // constructors
        /// <summary>
        /// Initializes a new instance of the BsonFactoryMethodMap class.
        /// </summary>
        /// <param name="classMap">The class map.</param>
        /// <param name="methodInfo">The method info.</param>
        public BsonFactoryMethodMap(BsonClassMap classMap, MethodInfo methodInfo)
            : base(classMap, methodInfo, CreateDelegate(methodInfo))
        {
            _methodInfo = methodInfo;
        }

        // public properties
        /// <summary>
        /// Gets the method info.
        /// </summary>
        public MethodInfo MethodInfo
        {
            get { return _methodInfo; }
        }

        // private static methods
        private static Delegate CreateDelegate(MethodInfo methodInfo)
        {
            // build and compile the following delegate:
            // (p1, p2, ...) => factoryMethod(p1, p2, ...)
            var parameters = methodInfo.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
            var body = Expression.Call(methodInfo, parameters);
            var lambda = Expression.Lambda(body, parameters);
            return lambda.Compile();
        }
    }
}
