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

using MongoDB.Driver;
using NUnit.Framework;

namespace MongoDB.DriverUnitTests
{
    [TestFixture]
    public class MongoCredentialsTests
    {
        [Test]
        public void TestCreateCredentials()
        {
            var credentials = MongoCredentials.CreateStrongestCredentials("db", "username", "password");
            Assert.AreEqual("username", credentials.Username);
            Assert.AreEqual("password", ((PasswordEvidence)credentials.Evidence).Password);
        }

        [Test]
        public void TestEquals()
        {
            var a = MongoCredentials.CreateStrongestCredentials("db", "user1", "password");
            var b = MongoCredentials.CreateStrongestCredentials("db", "user1", "password");
            var c = MongoCredentials.CreateStrongestCredentials("db", "user2", "password");
            var d = MongoCredentials.CreateStrongestCredentials("db", "user2", "password1");
            var n = (MongoCredentials)null;

            Assert.IsTrue(object.Equals(a, b));
            Assert.IsFalse(object.Equals(a, c));
            Assert.IsFalse(a.Equals(n));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(c.Equals(d));

            Assert.IsTrue(a == b);
            Assert.IsFalse(a == c);
            Assert.IsFalse(a == null);
            Assert.IsFalse(null == a);
            Assert.IsTrue(n == null);
            Assert.IsTrue(null == n);
            Assert.IsFalse(c == d);

            Assert.IsFalse(a != b);
            Assert.IsTrue(a != c);
            Assert.IsTrue(a != null);
            Assert.IsTrue(null != a);
            Assert.IsFalse(n != null);
            Assert.IsFalse(null != n);
            Assert.IsTrue(c != d);
        }
    }
}
