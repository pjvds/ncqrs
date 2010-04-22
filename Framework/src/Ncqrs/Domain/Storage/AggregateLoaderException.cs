using System;

namespace Ncqrs.Eventing.ServiceModel.Bus.Storage
{
    [Serializable]
    public class AggregateLoaderException : Exception
    {
        public AggregateLoaderException(string message) : base(message) { }
        public AggregateLoaderException(string message, Exception inner) : base(message, inner) { }
        protected AggregateLoaderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
