using System;
using System.Runtime.Serialization;

namespace Ncqrs.Domain.Storage
{
    [Serializable]
    public class AggregateLoaderException : Exception
    {
        public AggregateLoaderException(string message) : base(message) { }
        public AggregateLoaderException(string message, Exception inner) : base(message, inner) { }
        protected AggregateLoaderException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }
}
