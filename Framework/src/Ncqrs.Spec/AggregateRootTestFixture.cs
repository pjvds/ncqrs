using System;
using System.Collections.Generic;
using System.Linq;
using Ncqrs.Domain;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;

namespace Ncqrs.Spec
{
    [Specification]
    [TestFixture] // TODO: Testdriven.net debug runner doesn't recognize inhiret attributes. Use native for now.
    public abstract class AggregateRootTestFixture<TAggregateRoot> where TAggregateRoot : AggregateRoot
    {
        protected IAggregateRootCreationStrategy CreationStrategy { get; set; }

        protected TAggregateRoot AggregateRoot { get; set; }

        protected Exception CaughtException { get; private set; }

        protected List<UncommittedEvent> PublishedEvents { get; private set; }
        
        protected virtual IEnumerable<object> Given()
        {
            return null;
        }
        
        protected virtual void Finally() { }
        
        protected abstract void When();

        [Given]
        [SetUp] // TODO: Testdriven.net debug runner doesn't recognize inhiret attributes. Use native for now.
        public void Setup()
        {
            Guid commitId = Guid.NewGuid();
            Guid sourceId = Guid.NewGuid();
            CreationStrategy = new SimpleAggregateRootCreationStrategy();

            AggregateRoot = CreationStrategy.CreateAggregateRoot<TAggregateRoot>();
            PublishedEvents = new List<UncommittedEvent>();
            
            var history = Given();
            if(history != null)
            {
                long sequence = 0;
                var stream = Prepare.Events(history).ForSource(AggregateRoot.EventSourceId);
                AggregateRoot.InitializeFromHistory(stream);
            }

            try
            {
                AggregateRoot.EventApplied += (s, e) => PublishedEvents.Add(e.Event);
                When();
            }
            catch (Exception exception)
            {
                CaughtException = exception;
            }
            finally
            {
                Finally();
            }
        }
    }
}