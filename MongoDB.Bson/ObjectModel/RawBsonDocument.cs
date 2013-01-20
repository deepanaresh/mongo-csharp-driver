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
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoDB.Bson
{
    /// <summary>
    /// Represents a raw BSON document in byte format.
    /// </summary>
    [Serializable]
    [BsonSerializer(typeof(RawBsonDocumentSerializer))]
    public class RawBsonDocument
    {
        // private fields
        private byte[] _bytes;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RawBsonDocument"/> class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public RawBsonDocument(byte[] bytes)
        {
            // TODO: validate that the bytes are a valid BSON document
            _bytes = bytes;
        }

        // public properties
        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <value>
        /// The bytes.
        /// </value>
        public byte[] Bytes
        {
            get { return _bytes; }
        }
    }
}
