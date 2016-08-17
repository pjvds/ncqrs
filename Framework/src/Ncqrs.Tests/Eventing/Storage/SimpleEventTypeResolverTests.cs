using FluentAssertions;
using Ncqrs.Eventing.Storage;
using Xunit;

namespace Ncqrs.Tests.Eventing.Storage
{
    
    public class SimpleEventTypeResolverTests
    {
        private SimpleEventTypeResolver resolver = new SimpleEventTypeResolver();

        [Fact]
        public void Resolves_types_to_event_names()
        {
            var type = typeof(ILog);
            var result = resolver.EventNameFor(type);
            result.Should().Be(type.AssemblyQualifiedName);
        }

        [Fact]
        public void Resolves_event_names_to_types()
        {
            var type = typeof(ILog);
            var result = resolver.ResolveType(type.AssemblyQualifiedName);
            result.Should().Be(type);
        }
    }
}
