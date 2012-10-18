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
using System.Xml;

using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoDB.Bson.Serialization.Serializers
{
    /// <summary>
    /// Represents a serializer for Decimals.
    /// </summary>
    public class DecimalSerializer : BsonBaseSerializer
    {
        // private static fields
        private static DecimalSerializer __instance = new DecimalSerializer();

        // constructors
        /// <summary>
        /// Initializes a new instance of the DecimalSerializer class.
        /// </summary>
        public DecimalSerializer()
            : base(new RepresentationSerializationOptions(BsonType.String))
        {
        }

        // public static properties
        /// <summary>
        /// Gets an instance of the DecimalSerializer class.
        /// </summary>
        public static DecimalSerializer Instance
        {
            get { return __instance; }
        }

        // public methods
        /// <summary>
        /// Deserializes an object from a BsonReader.
        /// </summary>
        /// <param name="serializationConfig">The serialization config.</param>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <param name="nominalType">The nominal type of the object.</param>
        /// <param name="actualType">The actual type of the object.</param>
        /// <param name="options">The serialization options.</param>
        /// <returns>An object.</returns>
        public override object Deserialize(
            SerializationConfig serializationConfig,
            BsonReader bsonReader,
            Type nominalType,
            Type actualType,
            IBsonSerializationOptions options)
        {
            VerifyTypes(nominalType, actualType, typeof(decimal));
            var representationSerializationOptions = EnsureSerializationOptions<RepresentationSerializationOptions>(options);

            var bsonType = bsonReader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.Array:
                    var array = (BsonArray)BsonArraySerializer.Instance.Deserialize(serializationConfig, bsonReader, typeof(BsonArray), null);
                    var bits = new int[4];
                    bits[0] = array[0].AsInt32;
                    bits[1] = array[1].AsInt32;
                    bits[2] = array[2].AsInt32;
                    bits[3] = array[3].AsInt32;
                    return new decimal(bits);
                case BsonType.Double:
                    return representationSerializationOptions.ToDecimal(bsonReader.ReadDouble());
                case BsonType.Int32:
                    return representationSerializationOptions.ToDecimal(bsonReader.ReadInt32());
                case BsonType.Int64:
                    return representationSerializationOptions.ToDecimal(bsonReader.ReadInt64());
                case BsonType.String:
                    return XmlConvert.ToDecimal(bsonReader.ReadString());
                default:
                    var message = string.Format("Cannot deserialize Decimal from BsonType {0}.", bsonType);
                    throw new FileFormatException(message);
            }
        }

        /// <summary>
        /// Serializes an object to a BsonWriter.
        /// </summary>
        /// <param name="serializationConfig">The serialization config.</param>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="nominalType">The nominal type.</param>
        /// <param name="value">The object.</param>
        /// <param name="options">The serialization options.</param>
        public override void Serialize(
            SerializationConfig serializationConfig,
            BsonWriter bsonWriter,
            Type nominalType,
            object value,
            IBsonSerializationOptions options)
        {
            var decimalValue = (Decimal)value;
            var representationSerializationOptions = EnsureSerializationOptions<RepresentationSerializationOptions>(options);

            switch (representationSerializationOptions.Representation)
            {
                case BsonType.Array:
                    bsonWriter.WriteStartArray();
                    var bits = Decimal.GetBits(decimalValue);
                    bsonWriter.WriteInt32(bits[0]);
                    bsonWriter.WriteInt32(bits[1]);
                    bsonWriter.WriteInt32(bits[2]);
                    bsonWriter.WriteInt32(bits[3]);
                    bsonWriter.WriteEndArray();
                    break;
                case BsonType.Double:
                    bsonWriter.WriteDouble(representationSerializationOptions.ToDouble(decimalValue));
                    break;
                case BsonType.Int32:
                    bsonWriter.WriteInt32(representationSerializationOptions.ToInt32(decimalValue));
                    break;
                case BsonType.Int64:
                    bsonWriter.WriteInt64(representationSerializationOptions.ToInt64(decimalValue));
                    break;
                case BsonType.String:
                    bsonWriter.WriteString(XmlConvert.ToString(decimalValue));
                    break;
                default:
                    var message = string.Format("'{0}' is not a valid Decimal representation.", representationSerializationOptions.Representation);
                    throw new BsonSerializationException(message);
            }
        }
    }
}
