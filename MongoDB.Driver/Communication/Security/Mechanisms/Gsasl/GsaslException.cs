using System;
using System.Runtime.Serialization;

namespace MongoDB.Driver.Communication.Security.Mechanisms.Gsasl
{
    /// <summary>
    /// Thrown from a gsasl wrapped operation.
    /// </summary>
    [Serializable]
    public class GsaslException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GsaslException" /> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        public GsaslException(int errorCode)
        {
            HResult = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GsaslException" /> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public GsaslException(int errorCode, string message)
            : base(message)
        {
            HResult = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GsaslException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected GsaslException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}