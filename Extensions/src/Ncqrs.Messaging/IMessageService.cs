namespace Ncqrs.Messaging
{
   /// <summary>
   /// An entry point for message processing.
   /// </summary>
   public interface IMessageService
   {
      /// <summary>
      /// Exectes processing of the message.
      /// </summary>
      /// <param name="message">A message to be processed.</param>
      void Process(object message);
   }
}