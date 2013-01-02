/* Copyright 2010-2012 10gen Inc.
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
using MongoDB.Bson.Serialization.Attributes;
using NUnit.Framework;

namespace MongoDB.BsonUnitTests.Jira.CSharp648
{
    [TestFixture]
    public class CSharp648Tests
    {
        public class C
        {
            public int Id;
            public User User;
        }

        [BsonNoId]
        public class User
        {
            public int Id;
            public string Name;
        }

        [Test]
        public void TestNoId()
        {
            var c = new C
            {
                Id = 1,
                User = new User { Id = 2, Name = "John" }
            };
            var json = c.ToJson();
            var expected = "{ '_id' : 1, 'User' : { 'Id' : 2, 'Name' : 'John' } }".Replace("'", "\"");
            Assert.AreEqual(expected, json);
        }
    }
}
