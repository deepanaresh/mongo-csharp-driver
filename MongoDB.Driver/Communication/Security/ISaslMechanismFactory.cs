using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Internal;

namespace MongoDB.Driver.Communication.Security
{
    internal interface ISaslMechanismFactory
    {
        string Name { get; }

        ISaslMechanism Create(MongoConnection connection, MongoClientIdentity identity);
    }
}
