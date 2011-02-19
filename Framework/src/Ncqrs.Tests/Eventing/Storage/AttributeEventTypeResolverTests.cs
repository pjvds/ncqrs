using System;
using FluentAssertions;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Storage;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing.Storage
{
    [TestFixture]
    public class AttributeEventTypeResolverTests
    {
        private AttributeEventTypeResolver resolver;

        [SetUp]
        public void Setup()
        {
            resolver = new AttributeEventTypeResolver();
        }


        [Test]
        public void Resolves_types_to_event_names()
        {
            var type = typeof(FooEvent);
            resolver.AddEvent(type);

            var result = resolver.EventNameFor(type);
            result.Should().Be("foo");
        }

        [Test]
        public void Does_not_use_alias_when_resolving_event_name()
        {
            var type = typeof(AliasedFooEvent);
            resolver.AddEvent(type);

            var result = resolver.EventNameFor(type);
            result.Should().Be("foo");
        }

        [Test]
        public void Resolves_event_names_to_types()
        {
            var type = typeof(FooEvent);
            resolver.AddEvent(type);

            var result = resolver.ResolveType("foo");
            result.Should().Be(type);
        }

        [Test]
        public void Resolves_alias_to_type()
        {
            var type = typeof(AliasedFooEvent);
            resolver.AddEvent(type);
            
            var result = resolver.ResolveType("bar");
            result.Should().Be(type);
        }


        [Test]
        public void Does_not_error_when_adding_event_twice()
        {
            var type = typeof(FooEvent);
            resolver.AddEvent(type);
            resolver.AddEvent(type);
        }

        [Test]
        public void Throws_if_adding_events_with_same_name()
        {
            resolver.AddEvent(typeof(FooEvent));
            var ex = Assert.Throws<ArgumentException>(() => resolver.AddEvent(typeof(AliasedFooEvent)));
            ex.Message.Should().Be("Could not add event 'foo' for type 'Ncqrs.Tests.Eventing.Storage.AttributeEventTypeResolverTests+AliasedFooEvent' as the type 'Ncqrs.Tests.Eventing.Storage.AttributeEventTypeResolverTests+FooEvent' is already using this name.\r\nParameter name: type");
        }

        [Test]
        public void Throws_if_adding_aliases_that_exists()
        {
            resolver.AddEvent(typeof(BarEvent));
            var ex = Assert.Throws<ArgumentException>(() => resolver.AddEvent(typeof(AliasedFooEvent)));
            ex.Message.Should().Be("Could not add event 'bar' for type 'Ncqrs.Tests.Eventing.Storage.AttributeEventTypeResolverTests+AliasedFooEvent' as the type 'Ncqrs.Tests.Eventing.Storage.AttributeEventTypeResolverTests+BarEvent' is already using this name.\r\nParameter name: type");
        }

        [Test]
        public void Throws_if_event_does_not_have_a_name()
        {
            var ex = Assert.Throws<ArgumentException>(() => resolver.AddEvent(typeof(UnnamedEvent1)));
            ex.Message.Should().Be("No name found for event Ncqrs.Tests.Eventing.Storage.AttributeEventTypeResolverTests+UnnamedEvent1, specify an EventNameAttribute.");
        }

        [Test]
        public void Throws_if_event_name_is_empty()
        {
            var ex = Assert.Throws<ArgumentException>(() => resolver.AddEvent(typeof(UnnamedEvent2)));
            ex.Message.Should().Be("Type Ncqrs.Tests.Eventing.Storage.AttributeEventTypeResolverTests+UnnamedEvent2 does not have a name");
        }

        [Test]
        public void Aliases_do_not_count_as_a_name()
        {
            var ex = Assert.Throws<ArgumentException>(() => resolver.AddEvent(typeof(UnnamedEvent3)));
            ex.Message.Should().Be("No name found for event Ncqrs.Tests.Eventing.Storage.AttributeEventTypeResolverTests+UnnamedEvent3, specify an EventNameAttribute.");
        }


        private class NotAnEvent {}

        private class UnnamedEvent1 : Event {}

        [EventName("  ")]
        private class UnnamedEvent2 : Event {}

        [EventNameAlias("bar")]
        private class UnnamedEvent3 : Event {}

        [EventName("foo")]
        private class FooEvent : Event {}

        [EventName("foo")]
        [EventNameAlias("bar")]
        private class AliasedFooEvent : Event {}

        [EventName("bar")]
        private class BarEvent : Event {}
    }
}
