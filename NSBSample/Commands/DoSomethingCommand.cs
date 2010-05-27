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
   [MapsToAggregateRootMethod("Domain.SomeDomainObject, Domain","DoSomething")]
   public class DoSomethingCommand : CommandBase
   {
      [AggregateRootIdAttribute]
      public Guid ObjectId { get; set; }
      
      public string Value { get; set; }
   }
}
