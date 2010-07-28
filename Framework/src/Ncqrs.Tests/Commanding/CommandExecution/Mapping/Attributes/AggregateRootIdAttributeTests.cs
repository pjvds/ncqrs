using System;
using FluentAssertions;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using NUnit.Framework;

namespace Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes
{
    [TestFixture]
    public class AggregateRootIdAttributeTests
    {
        [Test]
        public void It_should_be_a_subclass_of_ExcludeInMappingAttribute()
        {
            var type = typeof (AggregateRootIdAttribute);
            typeof (ExcludeInMappingAttribute).IsAssignableFrom(type).Should().BeTrue();
        }
    }
}
