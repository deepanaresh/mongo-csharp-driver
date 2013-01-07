using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Internal;

namespace MongoDB.Driver.Communication.Security.Mechanisms
{
    internal class CramMD5MechanismFactory : ISaslMechanismFactory
    {
        public string Name
        {
            get { return "CRAM-MD5"; }
        }

        public ISaslMechanism Create(MongoConnection connection, MongoClientIdentity identity)
        {
            return new CramMD5Mechanism(identity);
        }
    }
}