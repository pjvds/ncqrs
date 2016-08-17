using System;
using FluentAssertions;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Storage;
using Xunit;

namespace Ncqrs.Tests.Eventing.Storage
{
    public class AttributeEventTypeResolverTests
    {
        private AttributeEventTypeResolver resolver;

        public AttributeEventTypeResolverTests()
        {
            resolver = new AttributeEventTypeResolver();
        }


        [Fact]
        public void Resolves_types_to_event_names()
        {
            var type = typeof(FooEvent);
            resolver.AddEvent(type);

            var result = resolver.EventNameFor(type);
            result.Should().Be("foo");
        }

        [Fact]
        public void Does_not_use_alias_when_resolving_event_name()
        {
            var type = typeof(AliasedFooEvent);
            resolver.AddEvent(type);

            var result = resolver.EventNameFor(type);
            result.Should().Be("foo");
        }

        [Fact]
        public void Resolves_event_names_to_types()
        {
            var type = typeof(FooEvent);
            resolver.AddEvent(type);

            var result = resolver.ResolveType("foo");
            result.Should().Be(type);
        }

        [Fact]
        public void Resolves_alias_to_type()
        {
            var type = typeof(AliasedFooEvent);
            resolver.AddEvent(type);
            
            var result = resolver.ResolveType("bar");
            result.Should().Be(type);
        }


        [Fact]
        public void Does_not_error_when_adding_event_twice()
        {
            var type = typeof(FooEvent);
            resolver.AddEvent(type);
            resolver.AddEvent(type);
        }

        [Fact]
        public void Throws_if_adding_events_with_same_name()
        {
            resolver.AddEvent(typeof(FooEvent));
            var ex = Assert.Throws<ArgumentException>(() => resolver.AddEvent(typeof(AliasedFooEvent)));
            ex.Should();
        }

        [Fact]
        public void Throws_if_adding_aliases_that_exists()
        {
            resolver.AddEvent(typeof(BarEvent));
            var ex = Assert.Throws<ArgumentException>(() => resolver.AddEvent(typeof(AliasedFooEvent)));
            ex.Should();
        }

        [Fact]
        public void Throws_if_event_does_not_have_a_name()
        {
            var ex = Assert.Throws<ArgumentException>(() => resolver.AddEvent(typeof(UnnamedEvent1)));
            ex.Message.Should().Be("No name found for event Ncqrs.Tests.Eventing.Storage.AttributeEventTypeResolverTests+UnnamedEvent1, specify an EventNameAttribute.");
        }

        [Fact]
        public void Throws_if_event_name_is_empty()
        {
            var ex = Assert.Throws<ArgumentException>(() => resolver.AddEvent(typeof(UnnamedEvent2)));
            ex.Message.Should().Be("Type Ncqrs.Tests.Eventing.Storage.AttributeEventTypeResolverTests+UnnamedEvent2 does not have a name");
        }

        [Fact]
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
