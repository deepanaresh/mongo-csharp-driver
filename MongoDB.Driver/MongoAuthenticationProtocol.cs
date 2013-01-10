
namespace MongoDB.Driver
{
    /// <summary>
    /// The protocol used to authenticate with MongoDB.
    /// </summary>
    public enum MongoAuthenticationProtocol
    {
        /// <summary>
        /// Authenticate to the server using GSSAPI.
        /// </summary>
        Gssapi,
        /// <summary>
        /// Authenticate to the server using the strongest means possible.
        /// </summary>
        Strongest
    }
}
