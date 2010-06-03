namespace Ncqrs.Messaging
{
   /// <summary>
   /// Represents requirements of particular message to be processed.
   /// </summary>
   public enum MessageProcessingRequirements
   {
      /// <summary>
      /// No requirements. The receiver aggregate can either exist or no 
      /// (in which case a new aggregate is created).
      /// </summary>
      None,
      /// <summary>
      /// Requires that the receiver aggregate exists.
      /// </summary>
      RequiresExisting,
      /// <summary>
      /// Requires that the receiver aggregate does not exist.
      /// </summary>
      RequiresNew
   }
}