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

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using NUnit.Framework;

namespace MongoDB.BsonUnitTests.Jira
{
    [TestFixture]
    public class CSharp476Tests
    {
        // [BsonImmutable]
        public class C
        {
            private int _x;
            private int _y;
            private int _z;

            // [BsonConstructor("X", "Y")]
            public C(int x, int y)
            {
                _x = x;
                _y = y;
            }

            public int X { get { return _x; } }
            public int Y { get { return _y; } }
            public int Z { get { return _z; } set { _z = value; } }
        }

        static CSharp476Tests()
        {
            BsonClassMap.RegisterClassMap<C>(cm =>
            {
                cm.SetIsImmutable(true);
                cm.MapMember(c => c.X);
                cm.MapMember(c => c.Y);
                cm.MapMember(c => c.Z);
                cm.MapConstructor(new [] { cm.GetMemberMap(c => c.X), cm.GetMemberMap(c => c.Y) });
            });
        }

        [Test]
        public void TestDeserialization()
        {
            var json = "{ X : 1, Y : 2, Z : 3 }";
            var c = BsonSerializer.Deserialize<C>(json);
            Assert.AreEqual(1, c.X);
            Assert.AreEqual(2, c.Y);
            Assert.AreEqual(3, c.Z);
        }

        [Test]
        public void TestSerialization()
        {
            var c = new C(1, 2) { Z = 3 };
            var expected = "{ 'X' : 1, 'Y' : 2, 'Z' : 3 }".Replace("'", "\"");
            Assert.AreEqual(expected, c.ToJson());
        }
    }
}
