using System;
using System.Runtime.Serialization;

namespace Janacek
{
    public class PatternParsingException : ApplicationException
    {
        public PatternParsingException()
        { }

        protected PatternParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public PatternParsingException(string message)
            : base(message)
        { }

        public PatternParsingException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
