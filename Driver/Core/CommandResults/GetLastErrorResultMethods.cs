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
using System.Text.RegularExpressions;

using MongoDB.Bson;

namespace MongoDB.Driver
{
    /// <summary>
    /// A temporary class to hold the GetLastErrorResult methods until SafeModeResult can be removed. These methods need to be
    /// in the base class of SafeModeResult otherwise they would not be accessible using an instance of SafeModeResult.
    /// </summary>
    [Serializable]
    [Obsolete("Use GetLastErrorResult instead (the GetLastErrorResultMethods class will be removed when SafeModeResult is removed).")]
    public class GetLastErrorResultMethods : CommandResult
    {
        // public properties
        /// <summary>
        /// Gets the number of documents affected.
        /// </summary>
        public long DocumentsAffected
        {
            get { return Response["n"].ToInt64(); }
        }

        /// <summary>
        /// Gets whether the result has a LastErrorMessage.
        /// </summary>
        public bool HasLastErrorMessage
        {
            get { return Response["err", false].ToBoolean(); }
        }

        /// <summary>
        /// Gets the last error message (null if none).
        /// </summary>
        public string LastErrorMessage
        {
            get
            {
                var err = Response["err", false];
                return (err.ToBoolean()) ? err.ToString() : null;
            }
        }

        /// <summary>
        /// Gets whether the last command updated an existing document.
        /// </summary>
        public bool UpdatedExisting
        {
            get
            {
                var updatedExisting = Response["updatedExisting", false];
                return updatedExisting.ToBoolean();
            }
        }
    }
}