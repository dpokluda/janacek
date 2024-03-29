﻿//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Janacek
{
    /// <summary>
    /// Exception for signalling pattern parsing errors.
    /// </summary>
    /// <seealso cref="T:System.ApplicationException"/>
    public class PatternParsingException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the Janacek.PatternParsingException class.
        /// </summary>
        public PatternParsingException()
        { }

        /// <summary>
        /// Initializes a new instance of the Janacek.PatternParsingException class.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Serialization context.</param>
        protected PatternParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// Initializes a new instance of the Janacek.PatternParsingException class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public PatternParsingException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the Janacek.PatternParsingException class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public PatternParsingException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
