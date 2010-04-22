using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;
using Ncqrs.Commands.Attributes;

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
