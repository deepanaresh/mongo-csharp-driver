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
using System.Collections.Generic;

namespace MongoDB.Driver.Communication.Security
{
    /// <summary>
    /// A high-level sasl conversation object.
    /// </summary>
    internal class SaslConversation : IDisposable
    {
        // private fields
        private bool _isDisposed;
        private List<IDisposable> _managedItemsNeedingDisposal;
        private List<IDisposable> _unmanagedItemsNeedingDisposal;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SaslConversation" /> class.
        /// </summary>
        public SaslConversation()
        {
            _managedItemsNeedingDisposal = new List<IDisposable>();
            _unmanagedItemsNeedingDisposal = new List<IDisposable>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SaslConversation" /> class.
        /// </summary>
        ~SaslConversation()
        {
            Dispose(false);
        }

        // public methods
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Registers a managed resource for disposal.
        /// </summary>
        /// <param name="disposable">The disposable.</param>
        public void RegisterManagedResourceForDisposal(IDisposable disposable)
        {
            _managedItemsNeedingDisposal.Add(disposable);
        }

        /// <summary>
        /// Registers an unmanaged resource for disposal.
        /// </summary>
        /// <param name="disposable">The disposable.</param>
        public void RegisterUnmanagedResourceForDisposal(IDisposable disposable)
        {
            _unmanagedItemsNeedingDisposal.Add(disposable);
        }

        // private methods
        private void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            // disposal should happen in reverse order of registration.
            if (disposing && _managedItemsNeedingDisposal != null)
            {
                for(int i = _managedItemsNeedingDisposal.Count - 1; i >= 0; i--)
                {
                    _managedItemsNeedingDisposal[i].Dispose();
                }

                _managedItemsNeedingDisposal.Clear();
                _managedItemsNeedingDisposal = null;
            }

            if (_unmanagedItemsNeedingDisposal != null)
            {
                for (int i = _unmanagedItemsNeedingDisposal.Count - 1; i >= 0; i--)
                {
                    _unmanagedItemsNeedingDisposal[i].Dispose();
                }

                _unmanagedItemsNeedingDisposal.Clear();
                _unmanagedItemsNeedingDisposal = null;
            }

            _isDisposed = true;
        }
    }
}