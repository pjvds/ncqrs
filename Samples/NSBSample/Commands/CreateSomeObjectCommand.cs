using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Commands
{
    [Serializable]
    [MapsToAggregateRootConstructor("Domain.SomeDomainObject, Domain")]
    public class CreateSomeObjectCommand : CommandBase
    {
        [Parameter(1)]
        public int DummyValue { get; set; }
    }
}