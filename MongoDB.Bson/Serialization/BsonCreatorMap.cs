﻿/* Copyright 2010-2013 10gen Inc.
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
    /// Represents a mapping to a delegate and its arguments.
    /// </summary>
    public class BsonCreatorMap
    {
        // private fields
        private readonly BsonClassMap _classMap;
        private readonly MemberInfo _memberInfo; // null if there is no corresponding constructor or factory method
        private readonly Delegate _delegate;
        private bool _isFrozen;
        private IEnumerable<MemberInfo> _arguments; // the members that define the values for the delegate's parameters

        // these values are set when Freeze is called
        private IEnumerable<string> _elementNames; // the element names of the serialized arguments
        private Dictionary<string, object> _defaultValues; // not all arguments have default values

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
        /// Gets the arguments.
        /// </summary>
        public IEnumerable<MemberInfo> Arguments
        {
            get { return _arguments; }
        }

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

        /// <summary>
        /// Gets the element names.
        /// </summary>
        public IEnumerable<string> ElementNames
        {
            get
            {
                if (!_isFrozen) { ThrowNotFrozenException(); }
                return _elementNames;
            }
        }

        /// <summary>
        /// Gets the member info (null if none).
        /// </summary>
        public MemberInfo MemberInfo
        {
            get { return _memberInfo; }
        }

        // public methods
        /// <summary>
        /// Freezes the creator map.
        /// </summary>
        public void Freeze()
        {
            if (!_isFrozen)
            {
                var allMemberMaps = _classMap.AllMemberMaps;

                var elementNames = new List<string>();
                var defaultValues = new Dictionary<string, object>();
                foreach (var argument in _arguments)
                {
                    // compare MetadataTokens because ReflectedTypes could be different (see p. 774-5 of C# 5.0 In a Nutshell)
                    var memberMap = allMemberMaps.FirstOrDefault(m => m.MemberInfo.MetadataToken == argument.MetadataToken);
                    if (memberMap == null)
                    {
                        var message = string.Format("Member '{0}' is not mapped.", argument.Name);
                        throw new BsonSerializationException(message);
                    }
                    elementNames.Add(memberMap.ElementName);
                    if (memberMap.IsDefaultValueSpecified)
                    {
                        defaultValues.Add(memberMap.ElementName, memberMap.DefaultValue);
                    }
                }

                _elementNames = elementNames;
                _defaultValues = defaultValues;
                _isFrozen = true;
            }
        }

        /// <summary>
        /// Gets whether there is a default value for a missing element.
        /// </summary>
        /// <param name="elementName">The element name.</param>
        /// <returns>True if there is a default value for element name; otherwise, false.</returns>
        public bool HasDefaultValue(string elementName)
        {
            if (!_isFrozen) { ThrowNotFrozenException(); }
            return _defaultValues.ContainsKey(elementName);
        }

        /// <summary>
        /// Sets the arguments for the creator map.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The creator map.</returns>
        public BsonCreatorMap SetArguments(IEnumerable<MemberInfo> arguments)
        {
            if (_isFrozen) { ThrowFrozenException(); }
            _arguments = new List<MemberInfo>(arguments);
            return this;
        }

        /// <summary>
        /// Sets the arguments for the creator map.
        /// </summary>
        /// <param name="argumentNames">The argument names.</param>
        /// <returns>The creator map.</returns>
        public BsonCreatorMap SetArguments(IEnumerable<string> argumentNames)
        {
            if (_isFrozen) { ThrowFrozenException(); }

            var arguments = new List<MemberInfo>();
            foreach (var argumentName in argumentNames)
            {
                var memberTypes = MemberTypes.Field | MemberTypes.Property;
                var bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                var memberInfos = _classMap.ClassType.GetMember(argumentName, memberTypes, bindingAttr);
                if (memberInfos.Length == 0)
                {
                    var message = string.Format("Class '{0}' does not have a member named '{1}'.", _classMap.ClassType.FullName, argumentName);
                    throw new BsonSerializationException(message);
                }
                else if (memberInfos.Length > 1)
                {
                    var message = string.Format("Class '{0}' has more than one member named '{1}'.", _classMap.ClassType.FullName, argumentName);
                    throw new BsonSerializationException(message);
                }
                arguments.Add(memberInfos[0]);
            }

            SetArguments(arguments);
            return this;
        }

        /// <summary>
        /// Try to get the default value for an argument.
        /// </summary>
        /// <param name="elementName">The missing element name.</param>
        /// <param name="defaultValue">The default value (if any).</param>
        /// <returns>True if there was a default value; otherwise, false.</returns>
        public bool TryGetDefaultValue(string elementName, out object defaultValue)
        {
            if (!_isFrozen) { ThrowNotFrozenException(); }
            return _defaultValues.TryGetValue(elementName, out defaultValue);
        }

        // private methods
        private void ThrowFrozenException()
        {
            throw new InvalidOperationException("BsonCreatorMap is frozen.");
        }

        private void ThrowNotFrozenException()
        {
            throw new InvalidOperationException("BsonCreatorMap is not frozen.");
        }
    }
}
