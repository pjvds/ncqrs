using System;

namespace Ncqrs.Messaging
{
   [Serializable]
   public class MessageProcessingRequirementsViolationException : Exception
   {
      public MessageProcessingRequirementsViolationException(IncomingMessage message)
         : base(GetText(message))
      {         
      }

      private static string GetText(IncomingMessage message)
      {
         return string.Format(message.ProcessingRequirements == MessageProcessingRequirements.RequiresExisting 
            ? "Expected to find existing aggregate with ID {0}" 
            : "Expected to find no aggregates with ID {0}", 
            message.ReceiverId);
      }
   }
}