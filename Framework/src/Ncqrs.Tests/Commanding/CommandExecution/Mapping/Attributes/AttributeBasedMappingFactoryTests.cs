using System;
using FluentAssertions;
using Ncqrs.Domain;
using NUnit.Framework;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding;

namespace Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes
{
    [TestFixture]
    public class AttributeBasedMappingFactoryTests
    {
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
        public void Creating_executor_with_runtime_determed_type_should_not_return_null()
        {
            var factory = new AttributeBasedMappingFactory();
            var commandType = typeof (CorrectlyMappedCommand);

            var result = factory.CreateExecutorForCommand(commandType);

            result.Should().NotBeNull();
        }

        [Test]
        public void Creating_executor_with_runtime_determed_type_should_create_working_executor()
        {
            var factory = new AttributeBasedMappingFactory();
            var commandType = typeof(CorrectlyMappedCommand);
            var executor = factory.CreateExecutorForCommand(commandType);

            var command = new CorrectlyMappedCommand { Bar = 25, Foo = "Hello world" };
            executor.Execute(command);

            TargetAggRoot.FooValue.Should().Be(command.Foo);
            TargetAggRoot.BarValue.Should().Be(command.Bar);
        }
    }
}
