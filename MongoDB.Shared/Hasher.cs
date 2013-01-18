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

using System.Collections;

namespace MongoDB.Shared
{
    internal class Hasher
    {
        // private fields
        private int _result;

        // constructors
        public Hasher()
        {
            _result = 17;
        }

        public Hasher(int seed)
        {
            _result = seed;
        }

        // public properties
        public int Result
        {
            get { return _result; }
        }

        // public methods
        // this overload added to avoid boxing
        public Hasher Hash(bool obj)
        {
            _result = 37 * _result + obj.GetHashCode();
            return this;
        }

        // this overload added to avoid boxing
        public Hasher Hash(int obj)
        {
            _result = 37 * _result + obj.GetHashCode();
            return this;
        }

        // this overload added to avoid boxing
        public Hasher Hash(long obj)
        {
            _result = 37 * _result + obj.GetHashCode();
            return this;
        }

        public Hasher Hash(object obj)
        {
            _result = 37 * _result + ((obj == null) ? 0 : obj.GetHashCode());
            return this;
        }

        public Hasher HashElements(IEnumerable sequence)
        {
            foreach (var obj in sequence)
            {
                Hash(obj);
            }
            return this;
        }
    }
}
