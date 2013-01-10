using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver
{
    /// <summary>
    /// Evidence of a MongoIdentity via the currently executing process.
    /// </summary>
    public sealed class ProcessEvidence : MongoIdentityEvidence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEvidence" /> class.
        /// </summary>
        public ProcessEvidence()
        { }
    }
}
