namespace Ncqrs.Messaging
{
   public interface IMessagingAggregateRoot
   {
      void ProcessMessage(IncomingMessage message);
   }
}