using System;
using System.Linq;
using FluentAssertions;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Eventing.Storage.SQL;
using NUnit.Framework;
using Rhino.Mocks;
using System.Data.SqlClient;
using Ncqrs.Eventing.Storage;
using System.Configuration;

namespace Ncqrs.Tests.Eventing.Storage.SQL
{
    [TestFixture]
    [Category("Integration")]
    public class MsSqlServerEventStoreTests
    {
        [Serializable]
        public class CustomerCreatedEvent : SourcedEvent
        {
            protected CustomerCreatedEvent()
            {
            }

            public CustomerCreatedEvent(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp, string name, int age)
                : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
                Name = name;
                Age = age;
            }

            public string Name
            { get; set; }

            public int Age
            {
                get;
                set;
            }
        }

        [Serializable]
        public class CustomerNameChanged : SourcedEvent
        {
            public Guid CustomerId
            {
                get
                {
                    return EventSourceId;
                }
            }

            public string NewName
            { get; set; }

            protected CustomerNameChanged()
            {

            }

            public CustomerNameChanged(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp, string newName)
                : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
                NewName = newName;
            }
        }

        [Serializable]
        public class AccountNameChangedEvent : SourcedEntityEvent
        {
            public Guid CustomerId { get { return EventSourceId; } }
            public Guid AccountId { get { return EntityId; } }
            public string NewAccountName { get; set; }

            public AccountNameChangedEvent()
            {

            }

            public AccountNameChangedEvent(Guid eventIdentifier, Guid aggregateRootId, Guid entityId, long eventSequence, DateTime eventTimeStamp, string newAccountName)
                : base(eventIdentifier, aggregateRootId, entityId, eventSequence, eventTimeStamp)
            {
                NewAccountName = newAccountName;
            }
        }

        [Serializable]
        public class MySnapshot : Snapshot
        {
        }

        private const string DEFAULT_CONNECTIONSTRING_KEY = "EventStore";
        private readonly string connectionString;
        private bool _ignored = false;

        public MsSqlServerEventStoreTests()
        {
            connectionString = ConfigurationManager.ConnectionStrings[DEFAULT_CONNECTIONSTRING_KEY].ConnectionString;
        }

        [SetUp]
        public void TestConnection()
        {
            var connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
            }
            catch (SqlException caught)
            {
                _ignored = true;
                Assert.Ignore("No connection could be made with SQL server: " + caught.Message);
            }
            finally
            {
                connection.Dispose();
            }
        }

        [TearDown]
        public void Clean()
        {
            if (!_ignored)
            {
                var connectionString = ConfigurationManager.ConnectionStrings[DEFAULT_CONNECTIONSTRING_KEY].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "TRUNCATE TABLE [Events]";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "TRUNCATE TABLE [EventSources]";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "TRUNCATE TABLE [Snapshots]";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [Test]
        public void Retrieving_table_creation_queries_should_return_dll()
        {
            var dllQueries = MsSqlServerEventStore.GetTableCreationQueries();

            dllQueries.Should().NotBeNull().And.NotBeEmpty();
        }

        [Test]
        public void Storing_event_source_should_succeed()
        {
            var targetStore = new MsSqlServerEventStore(connectionString);
            var theEventSourceId = Guid.NewGuid();
            var theInitialEventSourceVersion = 0;
            var theCommitId = Guid.NewGuid();
            var theVersion = new Version(1, 0);
            
            int sequenceCounter = 1;

            var events = new []
            {
                new UncommittedEvent
                (
                    Guid.NewGuid(), theEventSourceId, sequenceCounter++, theInitialEventSourceVersion, DateTime.Now, 
                    new CustomerCreatedEvent(Guid.NewGuid(), theEventSourceId, sequenceCounter, DateTime.UtcNow, "Foo",35),
                    theVersion
                ),
                new UncommittedEvent
                (
                    Guid.NewGuid(), theEventSourceId, sequenceCounter++, theInitialEventSourceVersion, DateTime.Now, 
                    new CustomerNameChanged(Guid.NewGuid(), theEventSourceId, sequenceCounter, DateTime.UtcNow, "Name"+sequenceCounter),
                    theVersion
                ),
                new UncommittedEvent
                (
                    Guid.NewGuid(), theEventSourceId, sequenceCounter++, theInitialEventSourceVersion, DateTime.Now, 
                    new CustomerNameChanged(Guid.NewGuid(), theEventSourceId, sequenceCounter, DateTime.UtcNow, "Name"+sequenceCounter),
                    theVersion
                ),
                new UncommittedEvent
                (
                    Guid.NewGuid(), theEventSourceId, sequenceCounter++, theInitialEventSourceVersion, DateTime.Now, 
                    new CustomerNameChanged(Guid.NewGuid(), theEventSourceId, sequenceCounter, DateTime.UtcNow, "Name"+sequenceCounter),
                    theVersion
                ),
            };

            var eventStream = new UncommittedEventStream(theCommitId);
            foreach(var e in events) eventStream.Append(e);

            targetStore.Store(eventStream);

            var eventsFromStore = targetStore.ReadUntil(theEventSourceId);
            eventsFromStore.Count().Should().Be(events.Count());

            for (int i = 0; i < eventsFromStore.Count(); i++)
            {
                var uncommittedEvent = events.ElementAt(i);
                var committedEvent = eventsFromStore.ElementAt(i);

                committedEvent.EventSourceId.Should().Be(uncommittedEvent.EventSourceId);
                committedEvent.EventIdentifier.Should().Be(uncommittedEvent.EventIdentifier);
                committedEvent.EventSequence.Should().Be(uncommittedEvent.EventSequence);
                committedEvent.EventTimeStamp.Should().Be(uncommittedEvent.EventTimeStamp);
                committedEvent.Payload.GetType().Should().Be(uncommittedEvent.Payload.GetType());
            }
        }

        //[Test]
        //public void Saving_event_source_while_there_is_a_newer_event_source_should_throw_concurency_exception()
        //{
        //    var targetStore = new MsSqlServerEventStore(connectionString);
        //    var id = Guid.NewGuid();

        //    int sequenceCounter = 0;

        //    var events = new SourcedEvent[]
        //                     {
        //                         new CustomerCreatedEvent(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Foo",
        //                                                  35),
        //                         new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow,
        //                                                 "Name" + sequenceCounter)
        //                     };

        //    var eventSource = MockRepository.GenerateMock<IEventSource>();
        //    eventSource.Stub(e => e.EventSourceId).Return(id).Repeat.Any();
        //    eventSource.Stub(e => e.InitialVersion).Return(0).Repeat.Any();
        //    eventSource.Stub(e => e.Version).Return(events.Length).Repeat.Any();
        //    eventSource.Stub(e => e.GetUncommittedEvents()).Return(events).Repeat.Any();

        //    targetStore.Save(eventSource);

        //    Action act = () => targetStore.Save(eventSource);
        //    act.ShouldThrow<ConcurrencyException>();
        //}

        //[Test]
        //public void Retrieving_all_events_should_return_the_same_as_added()
        //{
        //    var targetStore = new MsSqlServerEventStore(connectionString);
        //    var aggregateId = Guid.NewGuid();
        //    var entityId = Guid.NewGuid();

        //    int sequenceCounter = 1;
        //    var events = new SourcedEvent[]
        //                     {
        //                         new CustomerCreatedEvent(Guid.NewGuid(), aggregateId, sequenceCounter++, DateTime.UtcNow, "Foo",
        //                                                  35),
        //                         new CustomerNameChanged(Guid.NewGuid(), aggregateId, sequenceCounter++, DateTime.UtcNow,
        //                                                 "Name" + sequenceCounter),
        //                         new CustomerNameChanged(Guid.NewGuid(), aggregateId, sequenceCounter++, DateTime.UtcNow,
        //                                                 "Name" + sequenceCounter),
        //                         new CustomerNameChanged(Guid.NewGuid(), aggregateId, sequenceCounter++, DateTime.UtcNow,
        //                                                 "Name" + sequenceCounter),
        //                         new AccountNameChangedEvent(Guid.NewGuid(), aggregateId, entityId, sequenceCounter++, DateTime.UtcNow, "New Account Title" + sequenceCounter)
        //                     };

        //    var eventSource = MockRepository.GenerateMock<IEventSource>();
        //    eventSource.Stub(e => e.EventSourceId).Return(aggregateId);
        //    eventSource.Stub(e => e.InitialVersion).Return(0);
        //    eventSource.Stub(e => e.Version).Return(events.Length);
        //    eventSource.Stub(e => e.GetUncommittedEvents()).Return(events);

        //    targetStore.Save(eventSource);

        //    var result = targetStore.GetAllEvents(aggregateId);
        //    result.Count().Should().Be(events.Length);
        //    result.First().EventIdentifier.Should().Be(events.First().EventIdentifier);

        //    var foundEntityId = result.ElementAt(4).As<SourcedEntityEvent>().EntityId;
        //    foundEntityId.Should().Be(entityId);
        //}

        [Test]
        public void Saving_snapshot_should_not_throw_an_exception_when_snapshot_is_valid()
        {
            var targetStore = new MsSqlServerEventStore(connectionString);

            var anId = Guid.NewGuid();
            var aVersion = 12;
            var snapshot = new MySnapshot { EventSourceId = anId, EventSourceVersion = aVersion };

            targetStore.SaveShapshot(snapshot);

            var savedSnapshot = targetStore.GetSnapshot(anId);
            savedSnapshot.EventSourceId.Should().Be(anId);
            savedSnapshot.EventSourceVersion.Should().Be(aVersion);
        }
    }
}
