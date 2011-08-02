using System.Collections.Generic;

namespace Ncqrs.Messaging.Tests
{
    public class FakeSendingStrategy : ISendingStrategy
    {
        private readonly Queue<OutgoingMessage> _messages = new Queue<OutgoingMessage>();

        public void Send(OutgoingMessage message)
        {
            _messages.Enqueue(message);
        }

        public object DequeueMessage()
        {
            return _messages.Dequeue();
        }
    }
}