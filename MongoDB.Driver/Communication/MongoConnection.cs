﻿/* Copyright 2010-2013 10gen Inc.
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
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Communication;
using MongoDB.Driver.Communication.Security;

namespace MongoDB.Driver.Internal
{
    /// <summary>
    /// Represents the state of a connection.
    /// </summary>
    public enum MongoConnectionState
    {
        /// <summary>
        /// The connection has not yet been initialized.
        /// </summary>
        Initial,
        /// <summary>
        /// The connection is open.
        /// </summary>
        Open,
        /// <summary>
        /// The connection is closed.
        /// </summary>
        Closed
    }

    /// <summary>
    /// Represents a connection to a MongoServerInstance.
    /// </summary>
    public class MongoConnection
    {
        // private fields
        private object _connectionLock = new object();
        private MongoServerInstance _serverInstance;
        private MongoConnectionPool _connectionPool;
        private int _generationId; // the generationId of the connection pool at the time this connection was created
        private MongoConnectionState _state;
        private TcpClient _tcpClient;
        private Stream _stream; // either a NetworkStream or an SslStream wrapping a NetworkStream
        private DateTime _createdAt;
        private DateTime _lastUsedAt; // set every time the connection is Released
        private int _messageCounter;
        private int _requestId;
        private Authenticator _authenticator;

        // constructors
        internal MongoConnection(MongoConnectionPool connectionPool)
            : this(connectionPool.ServerInstance)
        {
            _connectionPool = connectionPool;
            _generationId = connectionPool.GenerationId;
        }

        internal MongoConnection(MongoServerInstance serverInstance)
        {
            _serverInstance = serverInstance;
            _createdAt = DateTime.UtcNow;
            _state = MongoConnectionState.Initial;
        }

        // public properties
        /// <summary>
        /// Gets the connection pool that this connection belongs to.
        /// </summary>
        public MongoConnectionPool ConnectionPool
        {
            get { return _connectionPool; }
        }

        /// <summary>
        /// Gets the DateTime that this connection was created at.
        /// </summary>
        public DateTime CreatedAt
        {
            get { return _createdAt; }
        }

        /// <summary>
        /// Gets the generation of the connection pool that this connection belongs to.
        /// </summary>
        public int GenerationId
        {
            get { return _generationId; }
        }

        /// <summary>
        /// Gets the DateTime that this connection was last used at.
        /// </summary>
        public DateTime LastUsedAt
        {
            get { return _lastUsedAt; }
            internal set { _lastUsedAt = value; }
        }

        /// <summary>
        /// Gets a count of the number of messages that have been sent using this connection.
        /// </summary>
        public int MessageCounter
        {
            get { return _messageCounter; }
        }

        /// <summary>
        /// Gets the RequestId of the last message sent on this connection.
        /// </summary>
        public int RequestId
        {
            get { return _requestId; }
        }

        /// <summary>
        /// Gets the server instance this connection is connected to.
        /// </summary>
        public MongoServerInstance ServerInstance
        {
            get { return _serverInstance; }
        }

        /// <summary>
        /// Gets the state of this connection.
        /// </summary>
        public MongoConnectionState State
        {
            get { return _state; }
        }

        // internal methods
        internal void Close()
        {
            lock (_connectionLock)
            {
                if (_state != MongoConnectionState.Closed)
                {
                    if (_stream != null)
                    {
                        try { _stream.Close(); } catch { } // ignore exceptions
                        _stream = null;
                    }
                    if (_tcpClient != null)
                    {
                        if (_tcpClient.Connected)
                        {
                            // even though MSDN says TcpClient.Close doesn't close the underlying socket
                            // it actually does (as proven by disassembling TcpClient and by experimentation)
                            try { _tcpClient.Close(); } catch { } // ignore exceptions
                        }
                        _tcpClient = null;
                    }
                    _state = MongoConnectionState.Closed;
                }
            }
        }

        internal bool IsExpired()
        {
            var now = DateTime.UtcNow;
            return now > _createdAt + _serverInstance.Settings.MaxConnectionLifeTime
                || now > _lastUsedAt + _serverInstance.Settings.MaxConnectionIdleTime;
        }

        internal void Open()
        {
            if (_state != MongoConnectionState.Initial)
            {
                throw new InvalidOperationException("Open called more than once.");
            }

            var ipEndPoint = _serverInstance.GetIPEndPoint();
            var tcpClient = new TcpClient(ipEndPoint.AddressFamily);
            tcpClient.NoDelay = true; // turn off Nagle
            tcpClient.ReceiveBufferSize = MongoDefaults.TcpReceiveBufferSize;
            tcpClient.SendBufferSize = MongoDefaults.TcpSendBufferSize;
            tcpClient.Connect(ipEndPoint);

            var stream = (Stream)tcpClient.GetStream();
            if (_serverInstance.Settings.UseSsl)
            {
                SslStream sslStream;
                if (_serverInstance.Settings.VerifySslCertificate)
                {
                    sslStream = new SslStream(stream, false); // don't leave inner stream open
                }
                else
                {
                    sslStream = new SslStream(stream, false, AcceptAnyCertificate, null); // don't leave inner stream open
                }

                try
                {
                    sslStream.AuthenticateAsClient(_serverInstance.Address.Host);
                }
                catch
                {
                    try { stream.Close(); }
                    catch { } // ignore exceptions
                    try { tcpClient.Close(); }
                    catch { } // ignore exceptions
                    throw;
                }
                stream = sslStream;
            }

            _tcpClient = tcpClient;
            _stream = stream;
            _state = MongoConnectionState.Open;

            new Authenticator(_serverInstance.Settings.CredentialsStore).Authenticate(this);
        }

        // this is a low level method that doesn't require a MongoServer
        // so it can be used while connecting to a MongoServer
        internal CommandResult RunCommand(
            string databaseName,
            QueryFlags queryFlags,
            CommandDocument command,
            bool throwOnError)
        {
            var commandName = command.GetElement(0).Name;

            var writerSettings = new BsonBinaryWriterSettings
            {
                GuidRepresentation = GuidRepresentation.Unspecified,
                MaxDocumentSize = _serverInstance.MaxDocumentSize
            };
            using (var message = new MongoQueryMessage(writerSettings, databaseName + ".$cmd", queryFlags, 0, 1, command, null))
            {
                SendMessage(message, null, databaseName); // write concern doesn't apply to queries
            }

            var readerSettings = new BsonBinaryReaderSettings
            {
                GuidRepresentation = GuidRepresentation.Unspecified,
                MaxDocumentSize = _serverInstance.MaxDocumentSize
            };
            var reply = ReceiveMessage<BsonDocument>(readerSettings, null);
            if (reply.NumberReturned == 0)
            {
                var message = string.Format("Command '{0}' failed. No response returned.", commandName);
                throw new MongoCommandException(message);
            }

            var commandResult = new CommandResult(command, reply.Documents[0]);
            if (throwOnError && !commandResult.Ok)
            {
                throw new MongoCommandException(commandResult);
            }

            return commandResult;
        }

        internal MongoReplyMessage<TDocument> ReceiveMessage<TDocument>(
            BsonBinaryReaderSettings readerSettings,
            IBsonSerializationOptions serializationOptions)
        {
            if (_state == MongoConnectionState.Closed) { throw new InvalidOperationException("Connection is closed."); }
            lock (_connectionLock)
            {
                try
                {
                    _lastUsedAt = DateTime.UtcNow;
                    using (var buffer = new BsonBuffer())
                    {
                        var networkStream = GetNetworkStream();
                        var readTimeout = (int)_serverInstance.Settings.SocketTimeout.TotalMilliseconds;
                        if (readTimeout != 0)
                        {
                            networkStream.ReadTimeout = readTimeout;
                        }
                        buffer.LoadFrom(networkStream);
                        var reply = new MongoReplyMessage<TDocument>(readerSettings);
                        reply.ReadFrom(buffer, serializationOptions);
                        return reply;
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                    throw;
                }
            }
        }

        internal WriteConcernResult SendMessage(MongoRequestMessage message, WriteConcern writeConcern, string databaseName)
        {
            if (_state == MongoConnectionState.Closed) { throw new InvalidOperationException("Connection is closed."); }
            lock (_connectionLock)
            {
                _lastUsedAt = DateTime.UtcNow;
                _requestId = message.RequestId;

                message.WriteToBuffer();
                CommandDocument getLastErrorCommand = null;
                if (writeConcern != null && writeConcern.Enabled)
                {
                    var fsync = (writeConcern.FSync == null) ? null : (BsonValue)writeConcern.FSync;
                    var journal = (writeConcern.Journal == null) ? null : (BsonValue)writeConcern.Journal;
                    var w = (writeConcern.W == null) ? null : writeConcern.W.ToGetLastErrorWValue();
                    var wTimeout = (writeConcern.WTimeout == null) ? null : (BsonValue)(int)writeConcern.WTimeout.Value.TotalMilliseconds;

                    getLastErrorCommand = new CommandDocument
                    {
                        { "getlasterror", 1 }, // use all lowercase for backward compatibility
                        { "fsync", fsync, fsync != null },
                        { "j", journal, journal != null },
                        { "w", w, w != null },
                        { "wtimeout", wTimeout, wTimeout != null }
                    };
                    // piggy back on network transmission for message
                    using (var getLastErrorMessage = new MongoQueryMessage(message.Buffer, message.WriterSettings, databaseName + ".$cmd", QueryFlags.None, 0, 1, getLastErrorCommand, null))
                    {
                        getLastErrorMessage.WriteToBuffer();
                    }
                }

                try
                {
                    var networkStream = GetNetworkStream();
                    var writeTimeout = (int)_serverInstance.Settings.SocketTimeout.TotalMilliseconds;
                    if (writeTimeout != 0)
                    {
                        networkStream.WriteTimeout = writeTimeout;
                    }
                    message.Buffer.WriteTo(networkStream);
                    _messageCounter++;
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                    throw;
                }

                WriteConcernResult writeConcernResult = null;
                if (writeConcern != null && writeConcern.Enabled)
                {
                    var readerSettings = new BsonBinaryReaderSettings
                    {
                        GuidRepresentation = message.WriterSettings.GuidRepresentation,
                        MaxDocumentSize = _serverInstance.MaxDocumentSize
                    };
                    var replyMessage = ReceiveMessage<BsonDocument>(readerSettings, null);
                    var getLastErrorResponse = replyMessage.Documents[0];
                    writeConcernResult = new WriteConcernResult();
                    writeConcernResult.Initialize(getLastErrorCommand, getLastErrorResponse);

                    if (!writeConcernResult.Ok)
                    {
                        var errorMessage = string.Format(
                            "WriteConcern detected an error '{0}'. (response was {1}).",
                            writeConcernResult.ErrorMessage, getLastErrorResponse.ToJson());
                        throw new WriteConcernException(errorMessage, writeConcernResult);
                    }
                    if (writeConcernResult.HasLastErrorMessage)
                    {
                        var errorMessage = string.Format(
                            "WriteConcern detected an error '{0}'. (Response was {1}).",
                            writeConcernResult.LastErrorMessage, getLastErrorResponse.ToJson());
                        throw new WriteConcernException(errorMessage, writeConcernResult);
                    }
                }

                return writeConcernResult;
            }
        }

        // private methods
        private bool AcceptAnyCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors
        )
        {
            return true;
        }

        private Stream GetNetworkStream()
        {
            if (_state == MongoConnectionState.Initial)
            {
                Open();
            }
            return _stream;
        }

        private void HandleException(Exception ex)
        {
            // there are three possible situations:
            // 1. we can keep using the connection
            // 2. just this one connection needs to be closed
            // 3. the whole connection pool needs to be cleared

            switch (DetermineAction(ex))
            {
                case HandleExceptionAction.KeepConnection:
                    break;
                case HandleExceptionAction.CloseConnection:
                    Close();
                    break;
                case HandleExceptionAction.ClearConnectionPool:
                    Close();
                    if (_connectionPool != null)
                    {
                        _connectionPool.Clear();
                    }
                    break;
                default:
                    throw new MongoInternalException("Invalid HandleExceptionAction");
            }

            // forces a call to VerifyState before the next message is sent to this server instance
            // this is a bit drastic but at least it's safe (and perhaps we can optimize a bit in the future)
            _serverInstance.SetState(MongoServerState.Unknown);
        }

        private enum HandleExceptionAction
        {
            KeepConnection,
            CloseConnection,
            ClearConnectionPool
        }

        private HandleExceptionAction DetermineAction(Exception ex)
        {
            // TODO: figure out when to return KeepConnection or ClearConnectionPool (if ever)

            // don't return ClearConnectionPool unless you are *sure* it is the right action
            // definitely don't make ClearConnectionPool the default action
            // returning ClearConnectionPool frequently can result in Connect/Disconnect storms

            return HandleExceptionAction.CloseConnection; // this should always be the default action
        }
    }
}
