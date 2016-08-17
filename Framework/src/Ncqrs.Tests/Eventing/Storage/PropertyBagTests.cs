using System;
using FluentAssertions;
using Ncqrs.Eventing.Storage;
using Xunit;

namespace Ncqrs.Tests.Eventing.Storage
{
    
    public class PropertyBagTests
    {
        [Fact]
        public void Creating_a_bag_should_not_throw_exception()
        {
            Action act = ()=>new PropertyBag("testing");
            act();
        }

        [Fact]
        public void EventName_information_should_match_initialized_name()
        {
            var bag = new PropertyBag("testing");
            bag.EventName.Should().Be("testing");
        }

        [Fact]
        public void An_new_instance_should_not_contain_any_properties()
        {
            var bag = new PropertyBag("testing");
            bag.Properties.Count.Should().Be(0);
        }

        [Fact]
        public void Calling_AddPropertyValue_should_add_the_property_info()
        {
            var thePropName = "MyProperty";
            var thePropValue = "Hello world";

            var bag = new PropertyBag("testing");
            bag.AddPropertyValue(thePropName, thePropValue);

            bag.Properties.Should().ContainKey(thePropName);
            bag.Properties.Should().ContainValue(thePropValue);
        }

        [Fact]
        public void The_number_of_properties_should_be_equal_to_the_number_of_calls_to_AddPropertyValue()
        {
            var callCount = 5;
            var bag = new PropertyBag("testing");

            for(int i =0; i<callCount; i++)
            {
                var name = "prop" + i;
                var value = i;

                bag.AddPropertyValue(name, value);
            }

            bag.Properties.Count.Should().Be(callCount);
        }

        [Fact]
        public void Adding_the_same_property_twice_should_cause_exception()
        {
            var propertyName = "MyProperty";
            var firstValue = 1;
            var secondValue = 2;

            var bag = new PropertyBag("testing");
            bag.AddPropertyValue(propertyName, firstValue);
            
            Action act = ()=>bag.AddPropertyValue(propertyName, secondValue);
            act.ShouldThrow<ArgumentException>();
        }
    }
}
