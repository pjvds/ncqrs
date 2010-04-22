using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Sample.Commands
{
    [Serializable]
    [MapsToAggregateRootMethod("Sample.Domain.Message, Sample.Domain", "UpdateMessageText")]
    public class UpdateMessageTextCommand : ICommand
    {
        [AggregateRootId]
        public Guid MessageId
        {
            get;set;
        }

        public String NewMessageText
        {
            get;set;
        }
    }
}
