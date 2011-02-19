using System;
using FluentAssertions;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.Domain;
using NUnit.Framework;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding;

namespace Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes
{
    [TestFixture]
    public class AttributeBasedCommandMapperTests
    {
        [MapsToAggregateRootConstructor("foo")]
        public class NonCommandTypeButWithCorrectAttribute
        {
        }

        public class CommandTypeButWithoutAttribute : ICommand
        {
            public Guid CommandIdentifier
            {
                get { throw new NotImplementedException(); }
            }

            public long? KnownVersion
            {
                get { throw new NotImplementedException(); }
            }
        }

        [MapsToAggregateRootConstructor("foo")]
        public class CommandTypeAndWithAttribute : ICommand
        {
            public Guid CommandIdentifier
            {
                get { throw new NotImplementedException(); }
            }

            public long? KnownVersion
            {
                get { throw new NotImplementedException(); }
            }
        }

        [MapsToAggregateRootConstructor("foo")]
        public class CommandTypeAndWithAttributeAndKnownVersion : ICommand
        {
            public Guid CommandIdentifier
            {
                get { throw new NotImplementedException(); }
            }

            public long? KnownVersion { get{ throw new NotImplementedException(); } }
        }

        public class TargetAggRoot : AggregateRootMappedByConvention
        {
            public static string FooValue;
            public static int BarValue;

            public TargetAggRoot(string foo, int bar)
            {
                FooValue = foo;
                BarValue = bar;
            }
        }

        [MapsToAggregateRootConstructor(typeof(TargetAggRoot))]
        public class CorrectlyMappedCommand : CommandBase
        {
            [Parameter]
            public string Foo
            {
                get; set;
            }

            [Parameter]
            public int Bar
            {
                get; set;
            }
        }

        [Test]
        public void Creating_executor_with_runtime_determed_type_should_create_working_executor()
        {
            var mapper = new AttributeBasedCommandMapper();
            var executor = new UoWMappedCommandExecutor(mapper);

            var command = new CorrectlyMappedCommand { Bar = 25, Foo = "Hello world" };
            executor.Execute(command);

            TargetAggRoot.FooValue.Should().Be(command.Foo);
            TargetAggRoot.BarValue.Should().Be(command.Bar);
        }

        [Test]
        public void IsCommandMapped_should_return_false_for_non_command_types()
        {
            var factory = new AttributeBasedCommandMapper();
            factory.CanMapCommand(typeof(string)).Should().BeFalse();
        }

        [Test]
        public void IsCommandMapped_should_return_false_for_non_command_types_that_do_have_the_required_attribute()
        {
            var factory = new AttributeBasedCommandMapper();
            factory.CanMapCommand(typeof(NonCommandTypeButWithCorrectAttribute)).Should().BeFalse();
        }

        [Test]
        public void IsCommandMapped_should_return_false_for_correct_command_types_but_that_does__not_have_the_required_attribute()
        {
            var factory = new AttributeBasedCommandMapper();
            factory.CanMapCommand(typeof(CommandTypeButWithoutAttribute)).Should().BeFalse();
        }

        [Test]
        public void IsCommandMapped_should_return_true_for_correct_command_types_with_correct_attribute()
        {
            var factory = new AttributeBasedCommandMapper();
            factory.CanMapCommand(typeof(CommandTypeAndWithAttribute)).Should().BeTrue();
        }
    }
}
