using System;
using FluentAssertions;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Xunit;

namespace Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes
{
    public class AggregateRootIdAttributeTests
    {
        [Fact]
        public void It_should_be_a_subclass_of_ExcludeInMappingAttribute()
        {
            var type = typeof (AggregateRootIdAttribute);
            typeof (ExcludeInMappingAttribute).IsAssignableFrom(type).Should().BeTrue();
        }
    }
}
