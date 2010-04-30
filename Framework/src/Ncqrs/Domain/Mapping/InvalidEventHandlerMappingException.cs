using System;
using System.Runtime.Serialization;

namespace Ncqrs.Domain.Mapping
{
    [Serializable]
    public class InvalidEventHandlerMappingException : Exception
    {
        public InvalidEventHandlerMappingException(string message) : base(message) { }
        public InvalidEventHandlerMappingException(string message, Exception inner) : base(message, inner) { }
        protected InvalidEventHandlerMappingException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }
}
