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
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;

namespace MongoDB.Bson.Serialization
{
    /// <summary>
    /// Represents the mapping between a constructor and a set of BSON elements.
    /// </summary>
    public class BsonConstructorParameter
    {
        // private fields
        private readonly Type _type;
        private readonly string _elementName;
        private readonly bool _hasDefaultValue;
        private readonly object _defaultValue;

        // constructors
        /// <summary>
        /// Initializes a new instance of the BsonConstructorParameter class.
        /// </summary>
        /// <param name="type">The parameter type.</param>
        /// <param name="elementName">The element name.</param>
        public BsonConstructorParameter(Type type, string elementName)
        {
            _type = type;
            _elementName = elementName;
        }

        /// <summary>
        /// Initializes a new instance of the BsonConstructorParameter class.
        /// </summary>
        /// <param name="type">The parameter type.</param>
        /// <param name="elementName">The element name.</param>
        /// <param name="defaultValue">The default value.</param>
        public BsonConstructorParameter(Type type, string elementName, object defaultValue)
        {
            _type = type;
            _elementName = elementName;
            _hasDefaultValue = true;
            _defaultValue = defaultValue;
        }

        // public properties
        /// <summary>
        /// Gets the default value.
        /// </summary>
        public object DefaultValue
        {
            get
            {
                if (!_hasDefaultValue)
                {
                    throw new NotSupportedException("There is no default value for this parameter.");
                }
                return _defaultValue;
            }
        }
        
        /// <summary>
        /// Gets the element name.
        /// </summary>
        public string ElementName
        {
            get { return _elementName; }
        }

        /// <summary>
        /// Gets whether the parameter has a default value.
        /// </summary>
        public bool HasDefaultValue
        {
            get { return _hasDefaultValue; }
        }
    
        /// <summary>
        /// Gets the parameter type.
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }
    }
}
