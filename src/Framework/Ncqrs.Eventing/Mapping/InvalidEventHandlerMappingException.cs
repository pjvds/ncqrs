using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Eventing.Mapping
{
    [Serializable]
    public class InvalidEventHandlerMappingException : Exception
    {
        public InvalidEventHandlerMappingException() { }
        public InvalidEventHandlerMappingException(string message) : base(message) { }
        public InvalidEventHandlerMappingException(string message, Exception inner) : base(message, inner) { }
        protected InvalidEventHandlerMappingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
