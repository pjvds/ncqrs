namespace Ncqrs.Messaging
{
   /// <summary>
   /// Defines interface for aggregate who wish to handle messages of certain type.
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public interface IMessageHandler<in T>
   {
      /// <summary>
      /// Executes the behavior of processing message of type <typeparamref name="T"/>.
      /// </summary>
      /// <param name="message">Message to be processed.</param>
      void Handle(T message);
   }
}