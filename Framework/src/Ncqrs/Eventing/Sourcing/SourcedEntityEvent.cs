using System;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.Eventing.Sourcing
{
    [Serializable]
    public class SourcedEntityEvent : SourcedEvent
    {
        public static Guid UndefinedEntityId = Guid.Empty;

        /// <summary>
        /// Gets the id of the entity that causes this event.
        /// </summary>
        public Guid EntityId { get; internal set; }


        public SourcedEntityEvent()
        {
        }

        public SourcedEntityEvent(Guid eventIdentifier, Guid eventSourceId, Guid entityId, long eventSequence, DateTime eventTimeStamp) : base(eventIdentifier, eventSourceId, eventSequence, eventTimeStamp)
        {
            EntityId = entityId;
        }

	public override void InitializeFrom(StoredEvent stored)
	{
		var evnt = stored as StoredEvent<Newtonsoft.Json.Linq.JObject>;
		if (evnt != null)
		{
			EntityId = new Guid(evnt.Data.Value<string>("EntityId"));
		}
		base.InitializeFrom(stored);
	}

	public void ClaimEvent(Guid eventSourceId, Guid entityId, long sequence)
	{
		EntityId = entityId;
		base.ClaimEvent(eventSourceId, sequence);			
	}
    }
}
