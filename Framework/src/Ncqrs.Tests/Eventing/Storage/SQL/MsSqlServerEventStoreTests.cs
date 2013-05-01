using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Eventing.Storage.SQL;
using Ncqrs.Spec;
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
        public class CustomerCreatedEvent
        {
            protected CustomerCreatedEvent()
            {
            }

            public CustomerCreatedEvent(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; set; }

            public int Age
            {
                get;
                set;
            }
        }

        [Serializable]
        public class CustomerNameChanged
        {
            public Guid CustomerId { get; set; }
            public string NewName { get; set; }

            protected CustomerNameChanged()
            {

            }

            public CustomerNameChanged(string newName)
            {
                NewName = newName;
            }
        }

        [Serializable]
        public class AccountNameChangedEvent : IEntitySourcedEvent
        {
            public Guid CustomerId { get; set; }
            public Guid AccountId { get; set; }
            public string NewAccountName { get; set; }

            public AccountNameChangedEvent()
            {

            }

            public AccountNameChangedEvent(Guid accountId, string newAccountName)
            {
                NewAccountName = newAccountName;
                AccountId = accountId;
            }

            public Guid EntityId
            {
                get { return AccountId; }
            }

            public Guid AggregateId
            {
                get { return CustomerId; }
            }
        }

        [Serializable]
        public class MySnapshot : Snapshot
        {
        }

        private const string DEFAULT_CONNECTIONSTRING_KEY = "EventStore";
        private readonly string connectionString;

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
                var cmd = connection.CreateCommand();
                cmd.CommandText = "IF EXISTS(SELECT * FROM sysobjects WHERE name='Events' AND xtype = 'U') TRUNCATE TABLE [Events]";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "IF EXISTS(SELECT * FROM sysobjects WHERE name='EventSources' AND xtype = 'U') TRUNCATE TABLE [EventSources]";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "IF EXISTS(SELECT * FROM sysobjects WHERE name='Snapshots' AND xtype = 'U') TRUNCATE TABLE [Snapshots]";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "IF EXISTS(SELECT * FROM sysobjects WHERE name='PipelineState' AND xtype = 'U') TRUNCATE TABLE [PipelineState]";
                cmd.ExecuteNonQuery();
            }
            catch (SqlException caught)
            {
                Assert.Ignore("No connection could be made with SQL server: " + caught.Message);
            }
            finally
            {
                connection.Dispose();
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
            var theCommitId = Guid.NewGuid();

            var eventStream = Prepare.Events(
                new CustomerCreatedEvent("Foo", 35),
                new CustomerNameChanged("Name" + 2),
                new CustomerNameChanged("Name" + 3),
                new CustomerNameChanged("Name" + 4))
                .ForSourceUncomitted(theEventSourceId, theCommitId);

            targetStore.Store(eventStream);

            var eventsFromStore = targetStore.ReadFrom(theEventSourceId, long.MinValue, long.MaxValue);
            eventsFromStore.Count().Should().Be(eventStream.Count());

            for (int i = 0; i < eventsFromStore.Count(); i++)
            {
                var uncommittedEvent = eventStream.ElementAt(i);
                var committedEvent = eventsFromStore.ElementAt(i);

                committedEvent.EventSourceId.Should().Be(uncommittedEvent.EventSourceId);
                committedEvent.EventIdentifier.Should().Be(uncommittedEvent.EventIdentifier);
                committedEvent.EventSequence.Should().Be(uncommittedEvent.EventSequence);
                committedEvent.Payload.GetType().Should().Be(uncommittedEvent.Payload.GetType());
            }
        }

        [Test]
        public void Storing_entity_sourced_event_should_preserve_entity_id()
        {
            var targetStore = new MsSqlServerEventStore(connectionString);
            var theEventSourceId = Guid.NewGuid();
            var theCommitId = Guid.NewGuid();

            var theEntityId = Guid.NewGuid();
            var eventStream = Prepare.Events(new AccountNameChangedEvent(theEntityId, "NewName"))
                .ForSourceUncomitted(theEventSourceId, theCommitId);

            targetStore.Store(eventStream);

            var restoredEvent = targetStore.ReadFrom(theEventSourceId, long.MinValue, long.MaxValue).Single();

            var payload = (AccountNameChangedEvent)restoredEvent.Payload;
            payload.EntityId.Should().Be(theEntityId);
        }

        [Test]
        public void Saving_with_concurrent_event_edits_should_be_subject_to_concurrency_checks()
        {
            // test created in response to an issue with concurrent edits happening within the window between
            // reading the current version number of the aggregate and the event source record being updated with
            // the new version number. this would leave the event stream for an event source out of sequence and
            // the aggregate in a state in which it could not be retrieved :o
            var concurrencyExceptionThrown = false;

            var targetStore = new MsSqlServerEventStore(connectionString);

            var theEventSourceId = Guid.NewGuid();
            var theCommitId = Guid.NewGuid();

            // make sure that the event source for the aggregate is created
            var creationEvent = Prepare.Events(new CustomerCreatedEvent("Foo", 35))
                .ForSourceUncomitted(theEventSourceId, theCommitId);
            targetStore.Store(creationEvent);

            var tasks = new Task[130];

            // now simulate concurreny updates coming in on the same aggregate
            for (int idx = 0; idx < tasks.Length; idx++)
            {
                tasks[idx] = Task.Factory.StartNew(() =>
                {

                    var changeEvent = new CustomerNameChanged(DateTime.Now.Ticks.ToString()) { CustomerId = theEventSourceId };
                    var eventStream = Prepare.Events(changeEvent)
                        .ForSourceUncomitted(theEventSourceId, Guid.NewGuid(), 2);

                    try
                    {
                        targetStore.Store(eventStream);

                    }
                    catch (ConcurrencyException)
                    {
                        concurrencyExceptionThrown = true;
                    }

                    targetStore.ReadFrom(theEventSourceId, long.MinValue, long.MaxValue);

                });
            }

            Task.WaitAll(tasks);

            if (concurrencyExceptionThrown == false)
            {
               Assert.Fail("We're expecting concurrency exceptions!");
            }
        }

        [Test]
        public void Saving_with_concurrent_event_adds_should_not_be_causing_deadlocks()
        {
            // test created in response to an issue with high frequency adds causing deadlocks on the EventSource table.
            // I reworked the sequencing of reads/updates to the EventSource table to reduce the amount
            // of time to any locks will be held. But this wasn't strictly required as the problem resided
            // in the fact that there was no index on the event source table result in full table scans occuring.
            // I therefore also changed the DDL to incude an non clustered index on EventSource.Id which resulted
            // in a nice performance boost during informal testing.

            var targetStore = new MsSqlServerEventStore(connectionString);

            var tasks = new Task[30]; // this number require to reproduce the issue might vary depending on hardware

            for (int idx = 0; idx < tasks.Length; idx++)
            {
                tasks[idx] = Task.Factory.StartNew(() =>
                {
                    var theEventSourceId = Guid.NewGuid();
                    var theCommitId = Guid.NewGuid();

                    var eventStream = Prepare.Events(new CustomerCreatedEvent(Task.CurrentId.ToString(), 35))
                        .ForSourceUncomitted(theEventSourceId, theCommitId);

                    // should not be receiving a deadlock
                    targetStore.Store(eventStream);
                });
            }

            Task.WaitAll(tasks);
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
            var snapshot = new Snapshot(anId, aVersion, new MySnapshot());

            targetStore.SaveSnapshot(snapshot);

            var savedSnapshot = targetStore.GetSnapshot(anId, long.MaxValue);
            savedSnapshot.EventSourceId.Should().Be(anId);
            savedSnapshot.Version.Should().Be(aVersion);
        }

        [Test]
        public void Storing_empty_event_stream_should_not_throw()
        {
            var targetStore = new MsSqlServerEventStore(connectionString);
            var theEventSourceId = Guid.NewGuid();
            var theCommitId = Guid.NewGuid();

            var eventStream = Prepare.Events(new object[0])
                .ForSourceUncomitted(theEventSourceId, theCommitId);

            targetStore.Store(eventStream);

            Assert.Pass();
        }



    }
}
