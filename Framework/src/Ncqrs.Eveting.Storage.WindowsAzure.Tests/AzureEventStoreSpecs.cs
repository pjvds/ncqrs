using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Storage.WindowsAzure.Tests
{
    [Serializable]
    public class FooEvent : DomainEvent
    {}

    [Serializable]
    public class BarEvent : DomainEvent
    {}

    [TestFixture]
    public class AzureEventStoreSpecs
    {
        [Test]
        public void All_the_events_from_a_new_event_source_should_add_events_to_event_store()
        {
            try
            {
                var mock = MockRepository.GenerateMock<IEventSource>();
                mock.Stub(m => mock.Id).Return(Guid.NewGuid());
                mock.Stub(m => mock.GetUncommittedEvents()).Return(new ISourcedEvent[] { new FooEvent(), new BarEvent() });

                var theEventStore = new AzureEventStore();
                theEventStore.Save(mock);

                var storedEvents = theEventStore.GetAllEventsForEventSource(mock.Id);
                storedEvents.Count().Should().Be(2);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [Test]
        public void All_the_events_from_a_event_source_should_add_events_to_event_store()
        {
            try
            {
                var mock = MockRepository.GenerateMock<IEventSource>();
                mock.Stub(m => mock.Id).Return(Guid.NewGuid());
                mock.Stub(m => mock.GetUncommittedEvents()).Return(new ISourcedEvent[] { new FooEvent(), new BarEvent() });

                var theEventStore = new AzureEventStore();
                theEventStore.Save(mock);

                mock.Stub(m => mock.Version).Return(2);
                mock.Stub(m => mock.GetUncommittedEvents()).Return(new ISourcedEvent[] { new FooEvent(), new BarEvent() });
                theEventStore.Save(mock);


                var storedEvents = theEventStore.GetAllEventsForEventSource(mock.Id);
                storedEvents.Count().Should().Be(4);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
