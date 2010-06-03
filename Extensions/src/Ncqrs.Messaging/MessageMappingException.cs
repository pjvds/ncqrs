using System;

namespace Ncqrs.Messaging
{
   [Serializable]
   public class MessageMappingException : Exception
   {
      public MessageMappingException(Type messageType, Type receiverType)
         : base(string.Format("Type {0} does not implement IMessageHandler interface for message type {1}",
         receiverType, messageType))
      {
         
      }
   }
}