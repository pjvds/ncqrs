using System;
using System.Collections.Generic;
using Ncqrs.Domain;
using Ncqrs.Domain.Storage;
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

        protected IEnumerable<ISourcedEvent> PublishedEvents { get; private set; }
        
        protected virtual IEnumerable<SourcedEvent> Given()
        {
            return null;
        }
        
        protected virtual void Finally() { }
        
        protected abstract void When();

        [Given]
        [SetUp] // TODO: Testdriven.net debug runner doesn't recognize inhiret attributes. Use native for now.
        public void Setup()
        {
            CreationStrategy = new SimpleAggregateRootCreationStrategy();

            AggregateRoot = CreationStrategy.CreateAggregateRoot<TAggregateRoot>();
            PublishedEvents = new SourcedEvent[0];
            
            var history = Given();
            if(history != null)
            {
                AggregateRoot.InitializeFromHistory(history);
            }

            try
            {
                When();
                PublishedEvents = AggregateRoot.GetUncommittedEvents();
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