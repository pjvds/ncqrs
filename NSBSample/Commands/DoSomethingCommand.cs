using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Commands
{
   [Serializable]
   [MapsToAggregateRootConstructor("Domain.SomeDomainObject, Domain")]
   public class DoSomethingCommand : CommandBase
   {
      [AggregateRootIdAttribute]
      public Guid ObjectId { get; set; }

      [ExcludeInMapping]
      public string Value { get; set; }
   }
}
