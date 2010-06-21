using System;
using AwesomeAppRefactored.Domain;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace AwesomeAppRefactored.Commands
{
    [MapsToAggregateRootMethod(typeof(Person), "ChangeName")]
    public class ChangeNameCommand : CommandBase
    {
        [AggregateRootId]
        public Guid PersonId { get; set; }
        public string Name { get; set; }

        public ChangeNameCommand()
        {
        }

        public ChangeNameCommand(Guid personId, string name)
        {
            PersonId = personId;
            Name = name;
        }
    }
}
