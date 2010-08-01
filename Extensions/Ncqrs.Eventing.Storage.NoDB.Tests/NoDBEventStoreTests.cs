using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage.NoDB.Tests.Fakes;
using Ncqrs.Eventing.Storage.Serialization;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Eventing.Storage.NoDB.Tests
{
    public class NoDBEventStoreTestFixture
    {
        protected NoDBEventStore EventStore;
        protected IEventSource Source;

        [SetUp]
        public void Setup()
        {
            EventStore = new NoDBEventStore("./");
            Source = MockRepository.GenerateMock<IEventSource>();
            Guid id = Guid.NewGuid();
            int sequenceCounter = 0;
            var events = new SourcedEvent[]
                             {
                                 new CustomerCreatedEvent(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Foo",35),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Name" + sequenceCounter),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Name" + sequenceCounter)
                             };
            Source.Stub(e => e.EventSourceId).Return(id);
            Source.Stub(e => e.InitialVersion).Return(0);
            Source.Stub(e => e.Version).Return(events.Length);
            Source.Stub(e => e.GetUncommittedEvents()).Return(events);
        }
    }

    public class when_saving_a_new_event_source : NoDBEventStoreTestFixture
    {
        private string _foldername;
        private string _filename;

        [SetUp]
        public void SetUp()
        {
            _foldername = Source.EventSourceId.ToString().Substring(0, 2);
            _filename = Source.EventSourceId.ToString().Substring(2);
            EventStore.Save(Source);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_foldername, true);
        }

        [Test]
        public void it_should_create_a_new_event_history_file()
        {
            Assert.That(File.Exists(Path.Combine(_foldername, _filename)));
        }

        [Test]
        public void it_should_serialize_the_uncommitted_events_to_the_file()
        {
            var formatter = new JsonEventFormatter(new SimpleEventTypeResolver());
            using (var reader = new StreamReader(File.Open(Path.Combine(_foldername, _filename), FileMode.Open)))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    Console.WriteLine(line);
                    var storedevent = line.ReadStoredEvent();
                    Assert.That(storedevent, Is.Not.Null);
                    line = reader.ReadLine();
                }
            }
        }
    }
}