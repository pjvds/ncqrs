using System;
using Ncqrs.Domain;

namespace Domain
{
    public class MyNotesAggregateRoot : AggregateRootMappedByConvention
    {
        public Guid Id
        {
            get
            {
                return EventSourceId;
            }
            set
            {
                EventSourceId = value;
            }
        }
    }
}
