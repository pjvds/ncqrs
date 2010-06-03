namespace Ncqrs.Messaging
{
   public interface IReceiverResolutionStrategy
   {
      ReceiverInfo Resolve(string receiverId);
   }
}