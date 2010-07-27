using System;
using System.Runtime.Serialization;

namespace Ncqrs.Domain.Storage
{
    [Serializable]
    public class AggregateRootNotFoundException : Exception
    {
        public AggregateRootNotFoundException()
        {
        }

        public AggregateRootNotFoundException(string message) : base(message)
        {
        }

        public AggregateRootNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AggregateRootNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
