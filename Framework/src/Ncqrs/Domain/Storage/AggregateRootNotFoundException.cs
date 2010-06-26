using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

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
