using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Internal;

namespace MongoDB.Driver.Communication.Security.Mechanisms
{
    internal class GssapiMechanismFactory : ISaslMechanismFactory
    {
        public string Name
        {
            get { return "GSSAPI"; }
        }

        public ISaslMechanism Create(MongoConnection connection, MongoClientIdentity identity)
        {
            // TODO: provide an override to force the use of gsasl.
            bool useGsasl = !Environment.OSVersion.Platform.ToString().Contains("Win");
            if (useGsasl)
            {
                return new GsaslGssapiMechanism(
                    connection.ServerInstance.Address.Host,
                    identity);
            }

            return new SspiMechanism(
                connection.ServerInstance.Address.Host,
                identity);
        }
    }
}