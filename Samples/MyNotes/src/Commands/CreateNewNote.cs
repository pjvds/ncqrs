using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Commands
{
    [MapsToAggregateRootConstructor("Domain.Note, Domain")]
    public class CreateNewNote : CommandBase
    {
        public String Text
        {
            get; set;
        }
    }
}
