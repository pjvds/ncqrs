using System;
using FluentAssertions;
using Ncqrs.Eventing.Storage;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing.Storage
{
    public class PropertyBagTests
    {
        [Test]
        public void Creating_a_bag_should_not_throw_exception()
        {
            Action act = ()=>new PropertyBag(typeof (object));
            act();
        }

        [Test]
        public void Type_information_should_match_initialized_type()
        {
            var type = typeof (object);
            var bag = new PropertyBag(type);

            bag.AssemblyName.Should().Be(type.Assembly.FullName);
            bag.AssemblyQualfiedName.Should().Be(type.AssemblyQualifiedName);
            bag.Namespace.Should().Be(type.Namespace);
            bag.TypeName.Should().Be(type.Name);
        }

        [Test]
        public void An_new_instance_should_not_contain_any_properties()
        {
            var bag = new PropertyBag(typeof (object));
            bag.Properties.Count.Should().Be(0);
        }

        [Test]
        public void Calling_AddPropertyValue_should_add_the_property_info()
        {
            var thePropName = "MyProperty";
            var thePropValue = "Hello world";

            var bag = new PropertyBag(typeof (object));
            bag.AddPropertyValue(thePropName, thePropValue);

            bag.Properties.Should().Contain(p => p.Key == thePropName && p.Value == thePropValue);
        }

        [Test]
        public void The_number_of_properties_should_be_equal_to_the_number_of_calls_to_AddPropertyValue()
        {
            var callCount = 5;
            var bag = new PropertyBag(typeof (object));

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

            var bag = new PropertyBag(typeof (object));
            bag.AddPropertyValue(propertyName, firstValue);
            
            Action act = ()=>bag.AddPropertyValue(propertyName, secondValue);
            act.ShouldThrow<ArgumentException>().WithMessage("An item with the same key has already been added.");
        }
    }
}
