using System;

namespace Ncqrs.Messaging
{
    public class MappingReceivingStrategy<T> : IReceivingStrategy
    {
        private readonly Func<T, IncomingMessage> _mappingFunction;

        public MappingReceivingStrategy(Func<T, IncomingMessage> mappingFunction)
        {
            _mappingFunction = mappingFunction;
        }

        public IncomingMessage Receive(object message)
        {
            return _mappingFunction((T) message);
        }
    }
}