﻿using System;
using System.Text;
using MongoDB.Driver.Communication.Security.Mechanisms.Sspi;

namespace MongoDB.Driver.Communication.Security.Mechanisms
{
    /// <summary>
    /// Implements the GSS API specification on Windows utilizing the native sspi libraries.
    /// </summary>
    internal class WindowsGssapiImplementation : ISaslStep
    {
        // private fields
        private readonly string _authorizationId;
        private readonly MongoIdentityEvidence _evidence;
        private readonly string _servicePrincipalName;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsGssapiImplementation" /> class.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="identity">The identity.</param>
        public WindowsGssapiImplementation(string serverName, string username, MongoIdentityEvidence evidence)
        {
            _authorizationId = username;
            _evidence = evidence;
            _servicePrincipalName = "mongodb/" + serverName;
        }

        // properties
        /// <summary>
        /// The bytes that should be sent to ther server before calling Transition.
        /// </summary>
        public byte[] BytesToSendToServer
        {
            get { return new byte[0]; }
        }

        // public methods
        /// <summary>
        /// Transitions to the next step in the conversation.
        /// </summary>
        /// <param name="conversation">The conversation.</param>
        /// <param name="bytesReceivedFromServer">The bytes received from the server.</param>
        /// <returns>An ISaslStep.</returns>
        public ISaslStep Transition(SaslConversation conversation, byte[] bytesReceivedFromServer)
        {
            SecurityCredentials securityCredentials;
            try
            {
                securityCredentials = SecurityCredentials.Acquire(SspiPackage.Kerberos, _authorizationId, _evidence);
                conversation.RegisterUnmanagedResourceForDisposal(securityCredentials);
            }
            catch (Win32Exception ex)
            {
                throw new MongoSecurityException("Unable to acquire security credentials.", ex);
            }

            byte[] bytesToSendToServer;
            SecurityContext context;
            try
            {
                context = SecurityContext.Initialize(securityCredentials, _servicePrincipalName, bytesReceivedFromServer, out bytesToSendToServer);
            }
            catch (Win32Exception ex)
            {
                if (_evidence is PasswordEvidence)
                {
                    throw new MongoSecurityException("Unable to initialize security context. Ensure the username and password are correct.", ex);
                }
                else
                {
                    throw new MongoSecurityException("Unable to initialize security context.", ex);
                }
            }

            if (!context.IsInitialized)
            {
                return new SspiInitializeStep(_servicePrincipalName, _authorizationId, context, bytesToSendToServer);
            }

            return new SspiNegotiateStep(_authorizationId, context, bytesToSendToServer);
        }

        // nested classes
        private class SspiInitializeStep : ISaslStep
        {
            private readonly string _authorizationId;
            private readonly SecurityContext _context;
            private readonly byte[] _bytesReceivedFromServer;
            private readonly string _servicePrincipalName;

            public SspiInitializeStep(string servicePrincipalName, string authorizationId, SecurityContext context, byte[] bytesToSendToServer)
            {
                _servicePrincipalName = servicePrincipalName;
                _authorizationId = authorizationId;
                _context = context;
                _bytesReceivedFromServer = bytesToSendToServer ?? new byte[0];
            }

            public byte[] BytesToSendToServer
            {
                get { return _bytesReceivedFromServer; }
            }

            public ISaslStep Transition(SaslConversation conversation, byte[] bytesReceivedFromServer)
            {
                byte[] bytesToSendToServer;
                try
                {
                    _context.Initialize(_servicePrincipalName, bytesReceivedFromServer, out bytesToSendToServer);
                }
                catch (Win32Exception ex)
                {
                    throw new MongoSecurityException("Unable to initialize security context", ex);
                }

                if (!_context.IsInitialized)
                {
                    return new SspiInitializeStep(_servicePrincipalName, _authorizationId, _context, bytesToSendToServer);
                }

                return new SspiNegotiateStep(_authorizationId, _context, bytesToSendToServer);
            }
        }

        private class SspiNegotiateStep : ISaslStep
        {
            private readonly string _authorizationId;
            private readonly SecurityContext _context;
            private readonly byte[] _bytesToSendToServer;

            public SspiNegotiateStep(string authorizationId, SecurityContext context, byte[] bytesToSendToServer)
            {
                _authorizationId = authorizationId;
                _context = context;
                _bytesToSendToServer = bytesToSendToServer ?? new byte[0];
            }

            public byte[] BytesToSendToServer
            {
                get { return _bytesToSendToServer; }
            }

            public ISaslStep Transition(SaslConversation conversation, byte[] bytesReceivedFromServer)
            {
                if (bytesReceivedFromServer == null || bytesReceivedFromServer.Length != 32) //RFC specifies this must be 4 octets
                {
                    throw new MongoSecurityException("Invalid server response.");
                }

                byte[] decryptedBytes;
                try
                {
                    _context.DecryptMessage(0, bytesReceivedFromServer, out decryptedBytes);
                }
                catch (Win32Exception ex)
                {
                    throw new MongoSecurityException("Unabled to decrypt message.", ex);
                }

                int length = 4;
                if (_authorizationId != null)
                {
                    length += _authorizationId.Length;
                }

                bytesReceivedFromServer = new byte[length];
                bytesReceivedFromServer[0] = 0x1; // NO_PROTECTION
                bytesReceivedFromServer[1] = 0x0; // NO_PROTECTION
                bytesReceivedFromServer[2] = 0x0; // NO_PROTECTION
                bytesReceivedFromServer[3] = 0x0; // NO_PROTECTION

                if (_authorizationId != null)
                {
                    var authorizationIdBytes = Encoding.UTF8.GetBytes(_authorizationId);
                    authorizationIdBytes.CopyTo(bytesReceivedFromServer, 4);
                }

                byte[] bytesToSendToServer;
                try
                {
                    _context.EncryptMessage(bytesReceivedFromServer, out bytesToSendToServer);
                }
                catch (Win32Exception ex)
                {
                    throw new MongoSecurityException("Unabled to encrypt message.", ex);
                }

                return new SaslCompletionStep(bytesToSendToServer);
            }
        }
    }
}