using System;
using System.Collections.Generic;
using System.Linq;
using Ncqrs.Domain;
using NUnit.Framework;
using FluentAssertions;

namespace Ncqrs.Tests.Domain
{
    public class EntityTests
    {
        public class MyEntityCretedEvent : DomainEvent
        {
            public Guid Id { get; set; }

            public MyEntityCretedEvent()
            {
            }

            public MyEntityCretedEvent(Guid id, Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp)
                : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
                Id = id;
            }
        }

        public class MyNestedEntityCretedEvent : DomainEvent
        {
            public Guid Id { get; set; }
        }

        public class NameUpdatedEvent : DomainEvent
        {
            public string Name { get; set; }

            public NameUpdatedEvent()
            {
            }

            public NameUpdatedEvent(string name, Guid eventIdentifier, Guid aggregateRootId, Guid entityId, long eventSequence, DateTime eventTimeStamp)
                : base(eventIdentifier, aggregateRootId, entityId, eventSequence, eventTimeStamp)
            {
                Name = name;
            }
        }

        public class ValueUpdatedEvent : DomainEvent
        {
            public int Value { get; set; }
        }

        public class MyAggregateRoot : AggregateRootMappedByConvention
        {
            public MyEntity Entity;

            public void CreateEntity(Guid externalId)
            {
                CreateEntity<MyEntity>(externalId);
                ApplyEvent(new MyEntityCretedEvent()
                               {
                                   Id = externalId
                               });
            }            

            private void OnMyEntityCretedEvent(MyEntityCretedEvent myEntityCretedEvent)
            {
                Entity = GetEntity<MyEntity>(myEntityCretedEvent.Id);
            }
        }

        public class MyEntity : EntityMappedByConvention
        {
            public string Name { get; set; }
            public MyNestedEntity ValueEntity { get; set; }

            public void UpdateName(string name)
            {
                ApplyEvent(new NameUpdatedEvent()
                               {
                                   Name = name
                               });
            }

            public void CreateValueEntity(Guid externalId)
            {
                AggregateRoot.CreateEntity<MyNestedEntity>(externalId);
                ApplyEvent(new MyNestedEntityCretedEvent()
                {
                    Id = externalId
                });
            }

            private void OnNameUpdatedEvent(NameUpdatedEvent nameUpdatedEvent)
            {
                Name = nameUpdatedEvent.Name;
            }

            private void OnNestedEntityCretedEvent(MyNestedEntityCretedEvent myNestedEntityCretedEvent)
            {
                ValueEntity = AggregateRoot.GetEntity<MyNestedEntity>(myNestedEntityCretedEvent.Id);
            }
        }

        public class MyNestedEntity : EntityMappedByConvention
        {
            public int Value { get; set; }

            public void UpdateValue(int value)
            {
                ApplyEvent(new ValueUpdatedEvent()
                {
                    Value = value
                });
            }

            private void OnValueUpdatedEvent(ValueUpdatedEvent valueUpdatedEvent)
            {
                Value = valueUpdatedEvent.Value;
            }
        }

        [Test]
        public void It_should_be_initialized_with_given_id()
        {
            using (var uow = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                var entityId = Guid.NewGuid();
                var root = new MyAggregateRoot();
                root.CreateEntity(entityId);
                root.Entity.Id.Should().Be(entityId);
            }
        }

        [Test]
        public void It_should_be_possible_to_call_methods_on_entities()
        {
            using (var uow = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                var entityId = Guid.NewGuid();
                var root = new MyAggregateRoot();
                root.CreateEntity(entityId);
                root.Entity.UpdateName("NewName");
                root.Entity.Name.Should().Be("NewName");
            }
        }

        [Test]
        public void It_should_be_possible_to_create_nested_entities()
        {
            using (var uow = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                var entityId = Guid.NewGuid();
                var nestedEntityId = Guid.NewGuid();
                var root = new MyAggregateRoot();
                root.CreateEntity(entityId);
                root.Entity.CreateValueEntity(nestedEntityId);
                root.Entity.ValueEntity.Should().NotBeNull();
            }
        }

        [Test]
        public void Applying_an_event_should_at_it_to_aggregates_uncommited_events()
        {
            using (NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                var theAggregate = new MyAggregateRoot();

                theAggregate.CreateEntity(Guid.NewGuid());

                theAggregate.GetUncommittedEvents().Count().Should().Be(2);

                theAggregate.Entity.UpdateName("NewName");

                theAggregate.GetUncommittedEvents().Count().Should().Be(3);
            }
        }

        [Test]
        public void Initiazling_from_history_should_recreate_entity()
        {
            var entityId = Guid.NewGuid();
            var theAggregate = new MyAggregateRoot();

            var event1 = new EntityCreatedEvent(entityId, typeof(MyEntity), new object[] {}, Guid.NewGuid(), theAggregate.Id, 1, DateTime.Now);
            var event2 = new MyEntityCretedEvent(entityId, Guid.NewGuid(), theAggregate.Id, 2, DateTime.Now);

            IEnumerable<DomainEvent> history = new DomainEvent[] { event1, event2 };

            theAggregate.InitializeFromHistory(history);
            theAggregate.Entity.Should().NotBeNull();
        }

        [Test]
        public void Events_created_by_entity_should_be_dispatched_to_entity_when_initializing_from_history()
        {
            var entityId = Guid.NewGuid();
            var theAggregate = new MyAggregateRoot();

            var event1 = new EntityCreatedEvent(entityId, typeof(MyEntity), new object[] { }, Guid.NewGuid(), theAggregate.Id, 1, DateTime.Now);
            var event2 = new MyEntityCretedEvent(entityId, Guid.NewGuid(), theAggregate.Id, 2, DateTime.Now);
            var event3 = new NameUpdatedEvent("NewName", Guid.NewGuid(), theAggregate.Id, entityId, 3, DateTime.Now);

            IEnumerable<DomainEvent> history = new DomainEvent[] { event1, event2, event3 };

            theAggregate.InitializeFromHistory(history);
            theAggregate.Entity.Name.Should().Be("NewName");
        }
    }
}