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

namespace MongoDB.Shared
{
    internal class CanonicalEquatableClass : IEquatable<CanonicalEquatableClass>
    {
        // private fields
        private int _x;
        private int _y;

        // constructors
        public CanonicalEquatableClass(int x, int y)
        {
            _x = y;
            _y = y;
        }

        // public operators
        public static bool operator ==(CanonicalEquatableClass lhs, CanonicalEquatableClass rhs)
        {
            return object.Equals(lhs, rhs); // handles lhs == null correctly
        }

        public static bool operator !=(CanonicalEquatableClass lhs, CanonicalEquatableClass rhs)
        {
            return !(lhs == rhs);
        }

        // public methods
        public bool Equals(CanonicalEquatableClass obj)
        {
            return Equals((object)obj); // handles obj == null correctly
        }

        // actual work done here (in virtual Equals) to handle inheritance
        public override bool Equals(object obj)
        {
            // use ReferenceEquals consistently because sometimes using == can lead to recursion loops
            // make sure to use GetType instead of typeof in case derived classes are involved
            if (object.ReferenceEquals(obj, null) || GetType() != obj.GetType()) { return false; }
            var rhs = (CanonicalEquatableClass)obj;
            return // be sure x and y implement ==, otherwise use Equals
                _x == rhs._x &&
                _y == rhs._y;
        }

        public override int GetHashCode()
        {
            return new Hasher()
                .Hash(_x)
                .Hash(_y)
                .Result;
        }
    }
}
