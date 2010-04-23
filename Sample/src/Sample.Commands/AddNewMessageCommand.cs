using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Sample.Commands
{
    [Serializable]
    [MapsToAggregateRootConstructor("Sample.Domain.Message, Sample.Domain")]
    public class AddNewMessageCommand : CommandBase
    {
        public Guid MessageId
        {
            get;
            set;
        }

        public String Text
        {
            get;
            set;
        }

        public AddNewMessageCommand()
        {
            MessageId = Guid.NewGuid();
        }
    }
}
