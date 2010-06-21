using System;
using AwesomeAppRefactored.Domain;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace AwesomeAppRefactored.Commands
{
    [MapsToAggregateRootConstructor(typeof(Person))]
    public class CreatePersonCommand : CommandBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public CreatePersonCommand()
        {
        }

        public CreatePersonCommand(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
