/* Copyright 2010-2013 10gen Inc.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using MongoDB.Shared;

namespace MongoDB.Driver
{
    /// <summary>
    /// Represents the settings for using SSL.
    /// </summary>
    public class SslSettings
    {
        // private fields
        private bool _checkCertificateRevocation = true;
        private X509CertificateCollection _clientCertificateCollection;
        private LocalCertificateSelectionCallback _clientCertificateSelectionCallback;
        private SslProtocols _enabledSslProtocols = SslProtocols.Default;
        private RemoteCertificateValidationCallback _serverCertificateValidationCallback;

        // the following fields are set when the SslSettings are froze
        private bool _isFrozen;
        private int _hashCode;

        // public properties
        /// <summary>
        /// Gets or sets whether to check for certificate revocation.
        /// </summary>
        public bool CheckCertificateRevocation
        {
            get { return _checkCertificateRevocation; }
            set
            {
                if (_isFrozen) { throw new InvalidOperationException("SslSettings is frozen."); }
                _checkCertificateRevocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the client certificates.
        /// </summary>
        public IEnumerable<X509Certificate> ClientCertificates
        {
            get { return ((IEnumerable)_clientCertificateCollection).Cast<X509Certificate>().Select(c => CloneCertificate(c)); }
            set
            {
                if (_isFrozen) { throw new InvalidOperationException("SslSettings is frozen."); }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _clientCertificateCollection = new X509CertificateCollection(value.Select(c => CloneCertificate(c)).ToArray());
            }
        }

        /// <summary>
        /// Gets or sets the client certificate selection callback.
        /// </summary>
        public LocalCertificateSelectionCallback ClientCertificateSelectionCallback
        {
            get { return _clientCertificateSelectionCallback; }
            set
            {
                if (_isFrozen) { throw new InvalidOperationException("SslSettings is frozen."); }
                _clientCertificateSelectionCallback = value;
            }
        }

        /// <summary>
        /// Gets or sets the enabled SSL protocols.
        /// </summary>
        public SslProtocols EnabledSslProtocols
        {
            get { return _enabledSslProtocols; }
            set
            {
                if (_isFrozen) { throw new InvalidOperationException("SslSettings is frozen."); }
                _enabledSslProtocols = value;
            }
        }

        /// <summary>
        /// Gets or sets the server certificate validation callback.
        /// </summary>
        public RemoteCertificateValidationCallback ServerCertificateValidationCallback
        {
            get { return _serverCertificateValidationCallback; }
            set
            {
                if (_isFrozen) { throw new InvalidOperationException("SslSettings is frozen."); }
                _serverCertificateValidationCallback = value;
            }
        }

        // internal properties
        internal X509CertificateCollection ClientCertificateCollection
        {
            get { return _clientCertificateCollection; }
        }

        // public methods
        /// <summary>
        /// Clones an SslSettings.
        /// </summary>
        /// <returns>The cloned SslSettings.</returns>
        public SslSettings Clone()
        {
            var clone = new SslSettings();
            clone._checkCertificateRevocation = _checkCertificateRevocation;
            clone._clientCertificateCollection = _clientCertificateCollection; // is immutable
            clone._clientCertificateSelectionCallback = _clientCertificateSelectionCallback;
            clone._enabledSslProtocols = _enabledSslProtocols;
            clone._serverCertificateValidationCallback = _serverCertificateValidationCallback;
            return clone;
        }

        /// <summary>
        /// Compares two SslSettings instances.
        /// </summary>
        /// <param name="obj">The other instance.</param>
        /// <returns>True if the two instances are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(SslSettings))
            {
                return false;
            }

            var rhs = (SslSettings)obj;
            return
                _checkCertificateRevocation == rhs._checkCertificateRevocation &&
                object.Equals(_clientCertificateCollection, rhs._clientCertificateCollection) &&
                object.Equals(_clientCertificateSelectionCallback, rhs._clientCertificateSelectionCallback) &&
                _enabledSslProtocols == rhs._enabledSslProtocols &&
                object.Equals(_serverCertificateValidationCallback, rhs._serverCertificateValidationCallback);
        }

        /// <summary>
        /// Freezes the settings.
        /// </summary>
        /// <returns>The frozen settings.</returns>
        public SslSettings Freeze()
        {
            if (!_isFrozen)
            {
                _hashCode = GetHashCode();
                _isFrozen = true;
            }
            return this;
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            if (_isFrozen)
            {
                return _hashCode;
            }

            return new Hasher()
                .Hash(_checkCertificateRevocation)
                .HashElements(_clientCertificateCollection)
                .Hash(_clientCertificateSelectionCallback)
                .Hash(_enabledSslProtocols)
                .Hash(_serverCertificateValidationCallback)
                .HashCode;
        }

        /// <summary>
        /// Returns a string representation of the settings.
        /// </summary>
        /// <returns>A string representation of the settings.</returns>
        public override string ToString()
        {
            var parts = new List<string>();
            parts.Add(string.Format("CheckCertificateRevocation={0}", _checkCertificateRevocation));
            if (_clientCertificateCollection != null)
            {
                parts.Add(string.Format("ClientCertificates=[{0}]", string.Join(",", ((IEnumerable)_clientCertificateCollection).Cast<X509Certificate>().Select(c => c.Subject).ToArray())));
            }
            if (_clientCertificateSelectionCallback != null)
            {
                parts.Add(string.Format("ClientCertificateSelectionCallback={0}", _clientCertificateSelectionCallback.Method.Name));
            }
            parts.Add(string.Format("EnabledProtocols={0}", _enabledSslProtocols));
            if (_serverCertificateValidationCallback != null)
            {
                parts.Add(string.Format("ServerCertificateValidationCallback={0}", _serverCertificateValidationCallback.Method.Name));
            }
            
            return string.Format("{{{0}}}", string.Join(",", parts.ToArray()));
        }

        // private methods
        private X509Certificate CloneCertificate(X509Certificate certificate)
        {
            var certificate2 = certificate as X509Certificate2;
            if (certificate2 != null)
            {
                return new X509Certificate2(certificate2);
            }
            else
            {
                return new X509Certificate(certificate);
            }
        }

        // nested classes
        private class X509CertificateCollectionComparer : IEqualityComparer<X509CertificateCollection>
        {
            public bool Equals(X509CertificateCollection lhs, X509CertificateCollection rhs)
            {
                if (lhs == null) { return rhs == null; }
                return ((IEnumerable)lhs).Cast<X509Certificate>().SequenceEqual(((IEnumerable)rhs).Cast<X509Certificate>());
            }

            public int GetHashCode(X509CertificateCollection obj)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("obj");
                }

                var hash = 17;
                foreach (X509Certificate certificate in obj)
                {
                    hash += 37 * hash + certificate.GetHashCode();
                }
                return hash;
            }
        }
    }
}
