namespace Ncqrs.Messaging
{
   public interface IMessagingAggregateRoot
   {
      void ProcessMessage(IMessage message);
   }
}