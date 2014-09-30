using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ncqrs.Messaging.Tests
{
    public class FakeSendingStrategy : ISendingStrategy
    {
        private readonly Queue<OutgoingMessage> _messages = new Queue<OutgoingMessage>();
        private MessageService messageService;

        public FakeSendingStrategy(MessageService messageService)
        {
            this.messageService = messageService;
        }

        public void Send(OutgoingMessage message)
        {
            Task.Factory.StartNew(() => messageService.Process(message));
            //_messages.Enqueue(message);
        }

        public object DequeueMessage()
        {
            return _messages.Dequeue();
        }
    }
}