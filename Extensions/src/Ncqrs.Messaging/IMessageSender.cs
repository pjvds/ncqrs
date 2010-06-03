using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Messaging
{
   public interface IMessageSender
   {
      bool TrySend(IMessage message);
   }
}
