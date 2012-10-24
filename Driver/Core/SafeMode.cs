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
using System.Linq;
using System.Text;

namespace MongoDB.Driver
{
    /// <summary>
    /// Represents the different safe modes that can be used.
    /// </summary>
    [Serializable]
    [Obsolete("Use WriteConcern instead.")]
    public class SafeMode : IEquatable<SafeMode>
    {
        // private static fields
        private static SafeMode __false = new SafeMode(WriteConcern.NetworkErrorsOnly);
        private static SafeMode __fsyncTrue = new SafeMode(WriteConcern.FSyncTrue);
        private static SafeMode __true = new SafeMode(WriteConcern.Errors);
        private static SafeMode __w2 = new SafeMode(WriteConcern.W2);
        private static SafeMode __w3 = new SafeMode(WriteConcern.W3);
        private static SafeMode __w4 = new SafeMode(WriteConcern.W4);

        // private fields
        private readonly WriteConcern _writeConcern;

        // constructors
        /// <summary>
        /// Creates a new instance of the SafeMode class.
        /// </summary>
        /// <param name="enabled">Whether safe mode is enabled.</param>
        public SafeMode(bool enabled)
            : this(enabled, false)
        {
        }

        /// <summary>
        /// Creates a new instance of the SafeMode class.
        /// </summary>
        /// <param name="enabled">Whether safe mode is enabled.</param>
        /// <param name="fsync">Whether the server should call fsync after each operation.</param>
        public SafeMode(bool enabled, bool fsync)
            : this(enabled, fsync, 0)
        {
        }

        /// <summary>
        /// Creates a new instance of the SafeMode class.
        /// </summary>
        /// <param name="enabled">Whether safe mode is enabled.</param>
        /// <param name="fsync">Whether the server should call fsync after each operation.</param>
        /// <param name="w">The number of write replications that should be completed before server returns.</param>
        public SafeMode(bool enabled, bool fsync, int w)
            : this(enabled, fsync, w, TimeSpan.Zero)
        {
        }

        /// <summary>
        /// Creates a new instance of the SafeMode class.
        /// </summary>
        /// <param name="enabled">Whether safe mode is enabled.</param>
        /// <param name="fsync">Whether the server should call fsync after each operation.</param>
        /// <param name="w">The number of write replications that should be completed before server returns.</param>
        /// <param name="wtimeout">The timeout for each operation.</param>
        public SafeMode(bool enabled, bool fsync, int w, TimeSpan wtimeout)
        {
            if (fsync && !enabled)
            {
                throw new ArgumentException("fsync cannot be true when SafeMode is not enabled.");
            }
            if (w != 0 && !enabled)
            {
                throw new ArgumentException("w cannot be non-zero when SafeMode is not enabled.");
            }
            if (wtimeout != TimeSpan.Zero && w == 0)
            {
                throw new ArgumentException("wtimeout cannot be non-zero when w is zero.");
            }

            if (enabled)
            {
                _writeConcern = new WriteConcern { Enabled = true, FSync = fsync, W = w, WTimeout = wtimeout };
            }
            else
            {
                _writeConcern = new WriteConcern { Enabled = false };
            }
        }

        /// <summary>
        /// Creates a new instance of the SafeMode class.
        /// </summary>
        /// <param name="w">The number of write replications that should be completed before server returns.</param>
        public SafeMode(int w)
            : this(true, false, w)
        {
        }

        /// <summary>
        /// Creates a new instance of the SafeMode class.
        /// </summary>
        /// <param name="w">The number of write replications that should be completed before server returns.</param>
        /// <param name="wtimeout">The timeout for each operation.</param>
        public SafeMode(int w, TimeSpan wtimeout)
            : this(true, false, w, wtimeout)
        {
        }

        /// <summary>
        /// Creates a new instance of the SafeMode class.
        /// </summary>
        /// <param name="other">Another SafeMode to initialize this one from.</param>
        public SafeMode(SafeMode other)
            : this(false)
        {
            _writeConcern = (other == null) ? new WriteConcern() : other.WriteConcern.Clone();
        }

        internal SafeMode(WriteConcern writeConcern)
        {
            _writeConcern = writeConcern;
        }

        // public operators
        /// <summary>
        /// Converts a SafeMode to a WriteConcern.
        /// </summary>
        /// <param name="safeMode">The SafeMode.</param>
        /// <returns>A WriteConcern.</returns>
        public static implicit operator WriteConcern(SafeMode safeMode)
        {
            return safeMode._writeConcern;
        }

        // public static properties
        /// <summary>
        /// Gets an instance of SafeMode with safe mode off.
        /// </summary>
        public static SafeMode False
        {
            get { return __false; }
        }

        /// <summary>
        /// Gets an instance of SafeMode with fsync=true.
        /// </summary>
        public static SafeMode FSyncTrue
        {
            get { return __fsyncTrue; }
        }

        /// <summary>
        /// Gets an instance of SafeMode with safe mode on.
        /// </summary>
        public static SafeMode True
        {
            get { return __true; }
        }

        /// <summary>
        /// Gets an instance of SafeMode with safe mode on and w=2.
        /// </summary>
        public static SafeMode W2
        {
            get { return __w2; }
        }

        /// <summary>
        /// Gets an instance of SafeMode with safe mode on and w=3.
        /// </summary>
        public static SafeMode W3
        {
            get { return __w3; }
        }

        /// <summary>
        /// Gets an instance of SafeMode with safe mode on and w=4.
        /// </summary>
        public static SafeMode W4
        {
            get { return __w4; }
        }

        // public properties
        /// <summary>
        /// Gets whether safe mode is enabled.
        /// </summary>
        public bool Enabled
        {
            get { return _writeConcern.Enabled; }
            set { _writeConcern.Enabled = value; }
        }

        /// <summary>
        /// Gets whether fsync is true.
        /// </summary>
        public bool FSync
        {
            get { return _writeConcern.FSync; }
            set { _writeConcern.FSync = value; }
        }

        /// <summary>
        /// Gets whether wait for journal commit is true.
        /// </summary>
        [Obsolete("Use Journal instead.")]
        public bool J
        {
            get { return Journal; }
            set { Journal = value; }
        }

        /// <summary>
        /// Gets whether wait for journal commit is true.
        /// </summary>
        public bool Journal
        {
            get { return _writeConcern.Journal; }
            set { _writeConcern.Journal = value; }
        }

        /// <summary>
        /// Gets the w value (the number of write replications that must complete before the server returns).
        /// </summary>
        public int W
        {
            get { return _writeConcern.W; }
            set { _writeConcern.W = value; }
        }

        /// <summary>
        /// Gets the w mode (the w mode determines which write replications must complete before the server returns).
        /// </summary>
        public string WMode
        {
            get { return _writeConcern.WMode; }
            set { _writeConcern.WMode = value; }
        }

        /// <summary>
        /// Gets the wtimeout value (the timeout before which the server must return).
        /// </summary>
        public TimeSpan WTimeout
        {
            get { return _writeConcern.WTimeout; }
            set { _writeConcern.WTimeout = value; }
        }

        // internal properties
        internal WriteConcern WriteConcern
        {
            get { return _writeConcern; }
        }

        // public operators
        /// <summary>
        /// Determines whether two specified SafeMode objects have different values.
        /// </summary>
        /// <param name="lhs">The first value to compare, or null.</param>
        /// <param name="rhs">The second value to compare, or null.</param>
        /// <returns>True if the value of lhs is different from the value of rhs; otherwise, false.</returns>
        public static bool operator !=(SafeMode lhs, SafeMode rhs)
        {
            return !SafeMode.Equals(lhs, rhs);
        }

        /// <summary>
        /// Determines whether two specified SafeMode objects have the same value.
        /// </summary>
        /// <param name="lhs">The first value to compare, or null.</param>
        /// <param name="rhs">The second value to compare, or null.</param>
        /// <returns>True if the value of lhs is the same as the value of rhs; otherwise, false.</returns>
        public static bool operator ==(SafeMode lhs, SafeMode rhs)
        {
            return SafeMode.Equals(lhs, rhs);
        }

        // public static methods
        /// <summary>
        /// Creates a SafeMode instance (or returns an existing instance).
        /// </summary>
        /// <param name="enabled">Whether safe mode is enabled.</param>
        /// <returns>A SafeMode instance.</returns>
        public static SafeMode Create(bool enabled)
        {
            return Create(enabled, false);
        }

        /// <summary>
        /// Creates a SafeMode instance (or returns an existing instance).
        /// </summary>
        /// <param name="enabled">Whether safe mode is enabled.</param>
        /// <param name="fsync">Whether fysnc is true.</param>
        /// <returns>A SafeMode instance.</returns>
        public static SafeMode Create(bool enabled, bool fsync)
        {
            return Create(enabled, fsync, 0);
        }

        /// <summary>
        /// Creates a SafeMode instance (or returns an existing instance).
        /// </summary>
        /// <param name="enabled">Whether safe mode is enabled.</param>
        /// <param name="fsync">Whether fysnc is true.</param>
        /// <param name="w">The number of write replications that should be completed before server returns.</param>
        /// <returns>A SafeMode instance.</returns>
        public static SafeMode Create(bool enabled, bool fsync, int w)
        {
            return Create(enabled, fsync, w, TimeSpan.Zero);
        }

        /// <summary>
        /// Creates a SafeMode instance (or returns an existing instance).
        /// </summary>
        /// <param name="enabled">Whether safe mode is enabled.</param>
        /// <param name="fsync">Whether fysnc is true.</param>
        /// <param name="w">The number of write replications that should be completed before server returns.</param>
        /// <param name="wtimeout">The timeout for each operation.</param>
        /// <returns>A SafeMode instance.</returns>
        public static SafeMode Create(bool enabled, bool fsync, int w, TimeSpan wtimeout)
        {
            if (!fsync && wtimeout == TimeSpan.Zero)
            {
                if (enabled)
                {
                    switch (w)
                    {
                        case 2: return __w2;
                        case 3: return __w3;
                        case 4: return __w4;
                        default: return new SafeMode(true, false, w);
                    }
                }
                else if (w == 0)
                {
                    return __false;
                }
            }
            return new SafeMode(enabled, fsync, w, wtimeout);
        }

        /// <summary>
        /// Creates a SafeMode instance (or returns an existing instance).
        /// </summary>
        /// <param name="w">The number of write replications that should be completed before server returns.</param>
        /// <returns>A SafeMode instance.</returns>
        public static SafeMode Create(int w)
        {
            return Create(w, TimeSpan.Zero);
        }

        /// <summary>
        /// Creates a SafeMode instance (or returns an existing instance).
        /// </summary>
        /// <param name="w">The number of write replications that should be completed before server returns.</param>
        /// <param name="wtimeout">The timeout for each operation.</param>
        /// <returns>A SafeMode instance.</returns>
        public static SafeMode Create(int w, TimeSpan wtimeout)
        {
            return Create(true, false, w, wtimeout);
        }

        /// <summary>
        /// Determines whether two specified SafeMode objects have the same value.
        /// </summary>
        /// <param name="lhs">The first value to compare, or null.</param>
        /// <param name="rhs">The second value to compare, or null.</param>
        /// <returns>True if the value of lhs is the same as the value of rhs; otherwise, false.</returns>
        public static bool Equals(SafeMode lhs, SafeMode rhs)
        {
            if ((object)lhs == null) { return (object)rhs == null; }
            return lhs.Equals(rhs);
        }

        // public methods
        /// <summary>
        /// Creates a clone of the SafeMode.
        /// </summary>
        /// <returns>A clone of the SafeMode.</returns>
        public SafeMode Clone()
        {
            return new SafeMode(_writeConcern.Clone());
        }

        /// <summary>
        /// Determines whether this instance and a specified object, which must also be a SafeMode object, have the same value.
        /// </summary>
        /// <param name="obj">The SafeMode object to compare to this instance.</param>
        /// <returns>True if obj is a SafeMode object and its value is the same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as SafeMode); // works even if obj is null or of a different type
        }

        /// <summary>
        /// Determines whether this instance and another specified SafeMode object have the same value.
        /// </summary>
        /// <param name="rhs">The SafeMode object to compare to this instance.</param>
        /// <returns>True if the value of the rhs parameter is the same as this instance; otherwise, false.</returns>
        public bool Equals(SafeMode rhs)
        {
            if ((object)rhs == null || GetType() != rhs.GetType()) { return false; }
            if ((object)this == (object)rhs) { return true; }
            return _writeConcern == rhs._writeConcern;
        }

        /// <summary>
        /// Freezes the SafeMode.
        /// </summary>
        /// <returns>The frozen SafeMode.</returns>
        public SafeMode Freeze()
        {
            _writeConcern.Freeze();
            return this;
        }

        /// <summary>
        /// Returns a frozen copy of the SafeMode.
        /// </summary>
        /// <returns>A frozen copy of the SafeMode.</returns>
        public SafeMode FrozenCopy()
        {
            if (_writeConcern.IsFrozen)
            {
                return this;
            }
            else
            {
                return Clone().Freeze();
            }
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return _writeConcern.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the SafeMode.
        /// </summary>
        /// <returns>A string representation of the SafeMode.</returns>
        public override string ToString()
        {
            return _writeConcern.ToString();
        }
    }
}
