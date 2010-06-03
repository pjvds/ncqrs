using System;

namespace Ncqrs.Messaging
{
   public struct ReceiverInfo
   {
      private readonly Guid _id;
      private readonly Type _type;

      public ReceiverInfo(Guid id, Type type) : this()
      {
         _id = id;
         _type = type;
      }

      public Type Type
      {
         get { return _type; }
      }

      public Guid Id
      {
         get { return _id; }
      }
   }
}