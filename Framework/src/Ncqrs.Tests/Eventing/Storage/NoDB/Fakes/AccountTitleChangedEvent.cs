using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.Fakes
{
    [Serializable]
    public class AccountTitleChangedEvent : SourcedEntityEvent
    {
        public AccountTitleChangedEvent()
        {
        }

        public AccountTitleChangedEvent(Guid eventIdentifier, Guid eventSourceId, Guid entityId, long eventSequence,
                                    DateTime eventTimeStamp, string newTitle)
            : base(eventIdentifier, eventSourceId, entityId, eventSequence, eventTimeStamp)
        {
            NewTitle = newTitle;
        }

        public string NewTitle { get; set; }


        public override bool Equals(object obj)
        {
            var other = obj as AccountTitleChangedEvent;
            if (other == null) return false;
            bool result = EventIdentifier.Equals(other.EventIdentifier) &&
                          EventSourceId.Equals(other.EventSourceId) &&
                          EntityId.Equals(other.EntityId) &&
                          EventSequence.Equals(other.EventSequence) &&
                          EventTimeStamp.Equals(other.EventTimeStamp) &&
                          NewTitle.Equals(other.NewTitle);
            return result;
        }
    }
}
