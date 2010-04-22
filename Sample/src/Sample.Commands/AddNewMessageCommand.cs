using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;
using Ncqrs.Commands.Attributes;

namespace Sample.Commands
{
    [Serializable]
    [MapsToAggregateRootConstructor("Sample.Domain.Message, Sample.Domain")]
    public class AddNewMessageCommand : ICommand
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
