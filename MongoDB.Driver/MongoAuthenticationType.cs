
namespace MongoDB.Driver
{
    /// <summary>
    /// The authentication type used to communicate with MongoDB.
    /// </summary>
    public enum MongoAuthenticationType
    {
        /// <summary>
        /// Authenticate to the server using GSSAPI.
        /// </summary>
        Gssapi,
        /// <summary>
        /// Authenticate to the server using a Negotiation.
        /// </summary>
        Negotiate
    }
}
