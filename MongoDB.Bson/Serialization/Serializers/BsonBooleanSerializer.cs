﻿/* Copyright 2010-2012 10gen Inc.
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
using System.IO;
using System.Linq;
using System.Text;

using MongoDB.Bson.IO;

namespace MongoDB.Bson.Serialization.Serializers
{
    /// <summary>
    /// Represents a serializer for BsonBooleans.
    /// </summary>
    public class BsonBooleanSerializer : BsonBaseSerializer
    {
        // private static fields
        private static Lazy<BsonBooleanSerializer> __instance = new Lazy<BsonBooleanSerializer>();

        // constructors
        /// <summary>
        /// Initializes a new instance of the BsonBooleanSerializer class.
        /// </summary>
        public BsonBooleanSerializer()
        {
        }

        // public static properties
        /// <summary>
        /// Gets an instance of the BsonBooleanSerializer class.
        /// </summary>
        public static BsonBooleanSerializer Instance
        {
            get { return __instance.Value; }
        }

        // public methods
        /// <summary>
        /// Deserializes an object from a BsonReader.
        /// </summary>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <param name="nominalType">The nominal type of the object.</param>
        /// <param name="actualType">The actual type of the object.</param>
        /// <param name="options">The serialization options.</param>
        /// <returns>An object.</returns>
        public override object Deserialize(
            BsonReader bsonReader,
            Type nominalType,
            Type actualType,
            IBsonSerializationOptions options)
        {
            VerifyTypes(nominalType, actualType, typeof(BsonBoolean));

            var bsonType = bsonReader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.Boolean:
                    return (BsonBoolean)bsonReader.ReadBoolean();
                default:
                    var message = string.Format("Cannot deserialize BsonBoolean from BsonType {0}.", bsonType);
                    throw new FileFormatException(message);
            }
        }

        /// <summary>
        /// Serializes an object to a BsonWriter.
        /// </summary>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="nominalType">The nominal type.</param>
        /// <param name="value">The object.</param>
        /// <param name="options">The serialization options.</param>
        public override void Serialize(
            BsonWriter bsonWriter,
            Type nominalType,
            object value,
            IBsonSerializationOptions options)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var bsonBoolean = (BsonBoolean)value;
            bsonWriter.WriteBoolean(bsonBoolean.Value);
        }
    }
}
