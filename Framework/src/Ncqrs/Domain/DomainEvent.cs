using System;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    [Serializable]
    public abstract class DomainEvent : IEventSourcedEvent
    {
        public Guid AggregateRootId
        {
            get; internal set;
        }

        Guid IEventSourcedEvent.EventSourceId
        {
            get
            {
                return AggregateRootId;
            }
        }

        public Guid EventIdentifier
        {
            get; private set;
        }

        public DateTime EventTimeStamp
        {
            get; private set;
        }

        protected DomainEvent()
        {
            AggregateRootId = Guid.Empty;

            var clock = NcqrsEnvironment.Get<IClock>();
            var idGenerator = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();

            EventTimeStamp = clock.UtcNow();
            EventIdentifier = idGenerator.GenerateNewId();
        }
    }
}