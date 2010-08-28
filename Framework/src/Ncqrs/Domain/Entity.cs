using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Domain
{
    /// <summary>
    /// The abstract concept of an entity -- an object living inside an aggregate with own thread of identity.
    /// </summary>
    public abstract class Entity
    {
        [NonSerialized]
        private readonly AggregateRoot _parent;

        protected Entity(AggregateRoot parent)
        {
            _parent = parent;
        }

        protected void RegisterHandler(ISourcedEventHandler handler)
        {
            _parent.RegisterHandler(handler);
        }

        protected void ApplyEvent(SourcedEvent evnt)
        {
            _parent.ApplyEvent(evnt);
        }
    }
}