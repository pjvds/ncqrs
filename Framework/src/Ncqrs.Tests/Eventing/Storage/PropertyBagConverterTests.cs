using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;
using NUnit.Framework;
using FluentAssertions;

namespace Ncqrs.Tests.Eventing.Storage
{
    [TestFixture]
    public class PropertyBagConverterTests
    {
        public class TestEvent
        {
            public string SomeString { get; set; }
        }

        [Test]
        public void Restoration_of_an_event_from_a_property_bag_containing_nulls_should_not_fail()
        {
            try
            {
                var converter = new PropertyBagConverter { TypeResolver = new SimpleEventTypeResolver() };
                
                var bag = new PropertyBag(typeof(TestEvent).AssemblyQualifiedName);
                bag.AddPropertyValue("SomeString", null);
                
                var obj = converter.Convert(bag);

                obj.Should().NotBeNull();
                obj.Should().BeOfType<TestEvent>();

                ((TestEvent) obj).SomeString.Should().BeNull();
            } 
            catch(Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }
    }
}