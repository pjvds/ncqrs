using System;

namespace Ncqrs.Messaging
{
    public class ConditionalReceivingStrategy
    {
        private readonly Func<object, bool> _condition;
        private readonly IReceivingStrategy _receivingStrategy;

        public ConditionalReceivingStrategy(Func<object, bool> condition, IReceivingStrategy receivingStrategy)
        {
            _condition = condition;
            _receivingStrategy = receivingStrategy;
        }

        public IncomingMessage Receive(object message)
        {
            if (!_condition(message))
            {
                return null;
            }
            return _receivingStrategy.Receive(message);
        }
    }
}