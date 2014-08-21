using System;
using System.Collections.Generic;
using INServiceBusICommand = NServiceBus.ICommand;
using ICommand = Ncqrs.Commanding.ICommand;

namespace Ncqrs.NServiceBus
{
   [Serializable]
    public class CommandMessage : INServiceBusICommand
   {
      public IEnumerable<ICommand> Payload { get; set; }
   }
}