using System.Collections.Generic;

namespace Ncqrs.Messaging
{
   public class InMemoryLocalMessageSender : IMessageSender
   {
      private readonly Queue<IMessage> _messages = new Queue<IMessage>();

      public bool TrySend(IMessage message)
      {
         if (!LocalResolutionStrategy.Matches(message.ReceiverId))
         {
            return false;
         }
         _messages.Enqueue(message);
         return true;
      }

      public bool ProcessNext()
      {
         if (_messages.Count == 0)
         {
            return false;
         }
         var message = _messages.Dequeue();
         new MessageService(new LocalResolutionStrategy()).Process(message);
         return true;
      }
   }
}