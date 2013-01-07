﻿
namespace MongoDB.Driver.Communication.Security
{
    internal interface IAuthenticationStore
    {
        void Authenticate(string databaseName, MongoCredentials credentials);

        bool CanAuthenticate(string databaseName, MongoCredentials credentials);

        bool IsAuthenticated(string databaseName, MongoCredentials credentials);

        void Logout(string databaseName);
    }
}