using System;
using AwesomeApp.Domain;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace AwesomeApp.Commands
{
    [MapsToAggregateRootMethod(typeof(Person), "ChangeName")]
    public class ChangeNameCommand : CommandBase
    {
        [AggregateRootId]
        public Guid PersonId { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }

        public ChangeNameCommand()
        {
        }

        public ChangeNameCommand(Guid personId, string forename, string surname)
        {
            PersonId = personId;
            Forename = forename;
            Surname = surname;
        }
    }
}
