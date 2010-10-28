using System;
using nsb = NServiceBus;

namespace Ncqrs.NServiceBus
{
    public interface ICommandMessage : nsb.IMessage
    {
        Ncqrs.Commanding.ICommand Payload { get; set; }
    }
}
