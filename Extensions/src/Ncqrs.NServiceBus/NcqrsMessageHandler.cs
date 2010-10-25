using System;
using Ncqrs.Commanding.ServiceModel;
using NServiceBus;

namespace Ncqrs.NServiceBus
{
    /// <summary>
    /// NServiceBus message handler for messages transporting Ncqrs commands.
    /// </summary>
    public class NcqrsMessageHandler : IHandleMessages<ICommandMessage>
    {
        /// <summary>
        /// Command service which is injected by NServiceBus infrastructure.
        /// </summary>
        public ICommandService CommandService { get; set; }

        public void Handle(ICommandMessage message)
        {
            CommandService.Execute(message.Payload);
        }
    }
}