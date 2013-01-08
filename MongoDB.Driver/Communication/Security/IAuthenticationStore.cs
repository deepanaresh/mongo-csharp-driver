
namespace MongoDB.Driver.Communication.Security
{
    /// <summary>
    /// A store for authentications for a MongoConnection.
    /// </summary>
    internal interface IAuthenticationStore
    {
        /// <summary>
        /// Authenticates the connection against the given database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="credentials">The credentials.</param>
        void Authenticate(string databaseName, MongoCredentials credentials);

        /// <summary>
        /// Determines whether the connection can be authenticated against the given database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="credentials">The credentials.</param>
        bool CanAuthenticate(string databaseName, MongoCredentials credentials);

        /// <summary>
        /// Determines whether the connection is currently authenticated against the given database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="credentials">The credentials.</param>
        bool IsAuthenticated(string databaseName, MongoCredentials credentials);

        /// <summary>
        /// Logouts the connection out of the given database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        void Logout(string databaseName);
    }
}