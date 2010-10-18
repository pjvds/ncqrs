using System;
using FluentAssertions;
using Ncqrs.Eventing.Storage;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing.Storage
{
    [TestFixture]
    public class PropertyBagTests
    {
        [Test]
        public void Creating_a_bag_should_not_throw_exception()
        {
            Action act = ()=>new PropertyBag("testing");
            act();
        }

        [Test]
        public void EventName_information_should_match_initialized_name()
        {
            var bag = new PropertyBag("testing");
            bag.EventName.Should().Be("testing");
        }

        [Test]
        public void An_new_instance_should_not_contain_any_properties()
        {
            var bag = new PropertyBag("testing");
            bag.Properties.Count.Should().Be(0);
        }

        [Test]
        public void Calling_AddPropertyValue_should_add_the_property_info()
        {
            var thePropName = "MyProperty";
            var thePropValue = "Hello world";

            var bag = new PropertyBag("testing");
            bag.AddPropertyValue(thePropName, thePropValue);

            bag.Properties.Should().Contain(p => p.Key == thePropName && (String)p.Value == thePropValue);
        }

        [Test]
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

        [Test]
        public void Adding_the_same_property_twice_should_cause_exception()
        {
            var propertyName = "MyProperty";
            var firstValue = 1;
            var secondValue = 2;

            var bag = new PropertyBag("testing");
            bag.AddPropertyValue(propertyName, firstValue);
            
            Action act = ()=>bag.AddPropertyValue(propertyName, secondValue);
            act.ShouldThrow<ArgumentException>().WithMessage("An item with the same key has already been added.");
        }
    }
}
