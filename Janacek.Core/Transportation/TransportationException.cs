using System;
using System.Runtime.Serialization;

namespace Janacek
{
    class TransportationException : ApplicationException
    {
        public TransportationException()
        { }

        protected TransportationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public TransportationException(string message)
            : base(message)
        { }

        public TransportationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
