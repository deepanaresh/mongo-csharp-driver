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
    internal class CanonicalEquatableDerivedClass : CanonicalEquatableClass, IEquatable<CanonicalEquatableDerivedClass>
    {
        // private fields
        private int _z;

        // constructors
        public CanonicalEquatableDerivedClass(int x, int y, int z)
            : base(x, y)
        {
            _z = z;
        }

        // base class defines == and !=

        // public methods
        public bool Equals(CanonicalEquatableDerivedClass obj)
        {
            return Equals((object)obj);
        }

        public override bool Equals(object obj)
        {
            // base class checks for obj == null and correct type
            if (!base.Equals(obj)) { return false; }
            var rhs = (CanonicalEquatableDerivedClass)obj;
            return // be sure z implements ==, otherwise use Equals
                _z == rhs._z;
        }

        public override int GetHashCode()
        {
            // use hash code of base class as seed to Hasher
            return new Hasher(base.GetHashCode())
                .Hash(_z)
                .Result;
        }
    }
}
