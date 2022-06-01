//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Janacek
{
    /// <summary>
    /// Exception for signalling transportation errors.
    /// </summary>
    /// <seealso cref="T:System.ApplicationException"/>
    public class TransportationException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the Janacek.TransportationException class.
        /// </summary>
        public TransportationException()
        { }

        /// <summary>
        /// Initializes a new instance of the Janacek.TransportationException class.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Serialization context.</param>
        protected TransportationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// Initializes a new instance of the Janacek.TransportationException class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public TransportationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the Janacek.TransportationException class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public TransportationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
