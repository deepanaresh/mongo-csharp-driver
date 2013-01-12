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
using System;

namespace MongoDB.BsonUnitTests.Jira
{
    [TestFixture]
    public class CSharp476Tests
    {
        public class C
        {
            private int _x;
            private int _y;

            public C(int x, int y)
            {
                _x = x;
                _y = y;
            }

            [BsonElement] // opt-in read-only property
            public int X { get { return _x; } }
            [BsonElement] // opt-in read-only property
            public int Y { get { return _y; } }
        }

        public class D : C
        {
            private int _z;

            //[BsonConstructor] // NamedParameterCreatorMapConvention will match the parameters to members
            //[BsonConstructor("X", "Y")]
            public D(int x, int y)
                : base(x, y)
            {
            }

            //[BsonFactoryMethod] // NamedParameterCreatorMapConvention will match the parameters to members
            //[BsonFactoryMethod("X", "Y")]
            public static D Create(int x, int y)
            {
                return new D(x, y);
            }

            public int Z { get { return _z; } set { _z = value; } }
        }

        static CSharp476Tests()
        {
            BsonClassMap.RegisterClassMap<C>(cm =>
            {
                cm.MapMember(c => c.X);
                cm.MapMember(c => c.Y);
            });

            BsonClassMap.RegisterClassMap<D>(cm =>
            {
                var ccm = (BsonClassMap<C>)BsonClassMap.LookupClassMap(typeof(C));

                cm.MapMember(d => d.Z);

                var constructorInfo = typeof(D).GetConstructor(new[] { typeof(int), typeof(int) });
                //cm.MapConstructor(constructorInfo, "X", "Y");

                var methodInfo = typeof(D).GetMethod("Create");
                //cm.MapFactoryMethod(methodInfo, "X", "Y");

                var @delegate = (Func<int, int, D>)((int x, int y) => { var a = x; var b = y; return D.Create(a, b); }); // arbitrary code
                //cm.MapCreator(@delegate, "X", "Y"); // example using a delegate
                //cm.MapCreator(d => new D(d.X, d.Y)); // example using a constructor
                cm.MapCreator(d => D.Create(d.X, d.Y)); // example using a factory method
            });
        }

        [Test]
        public void TestDeserialization()
        {
            var json = "{ X : 1, Y : 2, Z : 3 }";
            var d = BsonSerializer.Deserialize<D>(json);
            Assert.AreEqual(1, d.X);
            Assert.AreEqual(2, d.Y);
            Assert.AreEqual(3, d.Z);
        }

        [Test]
        public void TestSerialization()
        {
            var d = new D(1, 2) { Z = 3 };
            var expected = "{ 'X' : 1, 'Y' : 2, 'Z' : 3 }".Replace("'", "\"");
            Assert.AreEqual(expected, d.ToJson());
        }
    }
}
