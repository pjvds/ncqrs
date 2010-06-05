using System;

namespace Ncqrs.Messaging
{
    public class ConditionalSendingStrategy
    {
        private readonly Func<object, bool> _condition;
        private readonly ISendingStrategy _sendingStrategy;

        public ConditionalSendingStrategy(Func<object, bool> condition, ISendingStrategy sendingStrategy)
        {
            _condition = condition;
            _sendingStrategy = sendingStrategy;
        }

        public bool Send(OutgoingMessage message)
        {
            if (!_condition(message))
            {
                return false;
            }
            _sendingStrategy.Send(message);
            return true;
        }
    }
}