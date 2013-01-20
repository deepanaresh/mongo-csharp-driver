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

using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using NUnit.Framework;

namespace MongoDB.BsonUnitTests.Serialization
{
    [TestFixture]
    public class RawBsonDocumentSerializerTests
    {
        public class C
        {
            public int Id;
            public RawBsonDocument RawDocument;
        }

        [Test]
        public void TestNestedRawBsonDocument()
        {
            var nestedDocument = new BsonDocument("b", 2);
            var originalRawDocument = new RawBsonDocument(nestedDocument.ToBson());
            var c = new C { Id = 1, RawDocument = originalRawDocument };
            var bytes = c.ToBson();
            var r = BsonSerializer.Deserialize<C>(bytes);
            Assert.IsTrue(originalRawDocument.Bytes.SequenceEqual(r.RawDocument.Bytes));
        }

        [Test]
        public void TestTopLevelRawBsonDocument()
        {
            var originalDocument = new BsonDocument { { "_id", 1 }, { "b", 2 } };
            var originalBytes = originalDocument.ToBson();
            var rawDocument = BsonSerializer.Deserialize<RawBsonDocument>(originalBytes);
            var bytes = rawDocument.ToBson();
            var document = BsonSerializer.Deserialize<BsonDocument>(bytes);
            Assert.AreEqual(originalDocument, document);
        }
    }
}
