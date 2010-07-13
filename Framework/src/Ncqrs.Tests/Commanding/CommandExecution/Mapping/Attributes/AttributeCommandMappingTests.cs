using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;
using FluentAssertions;

namespace Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes
{
    public class AttributeCommandMappingTests
    {
        private ICommandService TheService
        { get; set; }

        public static AggregateRootTarget AggRoot
        { get; set; }

        public static ComplexAggregateRootTarget ComplexAggRoot
        { get; set; }

        private static AggregateRootTarget GetAggregateRoot()
        {
            return AggRoot ?? (AggRoot = new AggregateRootTarget("from GetAggregateRoot()"));
        }

        [MapsToAggregateRootMethod("Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes.AttributeCommandMappingTests+AggregateRootTarget, Ncqrs.Tests", "UpdateTitle")]
        public class AggregateRootTargetUpdateTitleCommand : CommandBase
        {
            public string Title
            { get; set; }

            [AggregateRootId]
            public Guid Id
            { get; set; }
        }

        public class AggregateRootTargetNotAMappedCommand : CommandBase
        {
            public string Title
            { get; set; }

            [AggregateRootId]
            public Guid Id
            { get; set; }
        }

        [MapsToAggregateRootConstructor("Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes.AttributeCommandMappingTests+AggregateRootTarget, Ncqrs.Tests")]
        public class AggregateRootTargetCreateNewCommand : CommandBase
        {
            public string Title
            { get; set; }

            [ExcludeInMapping]
            public Guid Id
            { get; set; }
        }

        public class AggregateRootTargetTitleUpdatedEvent : SourcedEvent
        {
            public string Title
            { get; set; }
        }

        public class AggregateRootTargetCreatedNewEvent : SourcedEvent
        {
            public string Title
            { get; set; }
        }

        public class AggregateRootTarget : AggregateRootMappedWithExpressions
        {
            public string Title
            { get; private set; }

            public AggregateRootTarget(string title)
            {
                var eventargs = new AggregateRootTargetCreatedNewEvent { Title = title };
                ApplyEvent(eventargs);
            }

            private AggregateRootTarget()
            { }

            public void UpdateTitle(string title)
            {
                var e = new AggregateRootTargetTitleUpdatedEvent { Title = title };
                ApplyEvent(e);
            }

            public void TitleUpdated(AggregateRootTargetTitleUpdatedEvent ev)
            {
                this.Title = ev.Title;
                AggRoot = this;
            }

            private void NewTestAggregateRootCreated(AggregateRootTargetCreatedNewEvent ev)
            {
                Title = ev.Title;
                AggRoot = this;
            }

            public override void InitializeEventHandlers()
            {
                Map<AggregateRootTargetTitleUpdatedEvent>().ToHandler(eventargs => TitleUpdated(eventargs));
                Map<AggregateRootTargetCreatedNewEvent>().ToHandler(eventargs => NewTestAggregateRootCreated(eventargs));
            }
        }

        [MapsToAggregateRootConstructor("Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes.AttributeCommandMappingTests+ComplexAggregateRootTarget, Ncqrs.Tests")]
        public class ComplexAggregateRootTargetCreateNewCommand1 : CommandBase
        {
            [Parameter(1)]
            public string Title
            { get; set; }

            [Parameter(2)]
            public int Quantity
            { get; set; }
        }

        [MapsToAggregateRootConstructor("Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes.AttributeCommandMappingTests+ComplexAggregateRootTarget, Ncqrs.Tests")]
        public class ComplexAggregateRootTargetCreateNewCommand2 : CommandBase
        {
            [Parameter("title")]
            public string Title
            { get; set; }

            [Parameter("quantity")]
            public int Quantity
            { get; set; }
        }

        [MapsToAggregateRootConstructor("Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes.AttributeCommandMappingTests+ComplexAggregateRootTarget, Ncqrs.Tests")]
        public class ComplexAggregateRootTargetCreateNewCommand3 : CommandBase
        {
            [Parameter("title")]
            public string Title
            { get; set; }

            [Parameter(2)]
            public int Quantity
            { get; set; }
        }

        [MapsToAggregateRootConstructor("Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes.AttributeCommandMappingTests+ComplexAggregateRootTarget, Ncqrs.Tests")]
        public class ComplexAggregateRootTargetCreateNewCommand4 : CommandBase
        {
            [Parameter]
            public string Title
            { get; set; }

            [Parameter]
            public int Quantity
            { get; set; }
        }

        public class ComplexAggregateRootTargetCreatedNewEvent : SourcedEvent
        {
            public string Title
            { get; set; }

            public int Quantity
            { get; set; }
        }

        public class ComplexAggregateRootTarget : AggregateRootMappedWithExpressions
        {
            public string Title
            { get; private set; }

            public int Quantity
            { get; private set; }

            public ComplexAggregateRootTarget(string title, int quantity)
            {
                var eventargs = new ComplexAggregateRootTargetCreatedNewEvent { Title = title, Quantity = quantity };
                ApplyEvent(eventargs);
            }

            private ComplexAggregateRootTarget()
            { }

            private void NewTestComplexAggregateRootCreated(ComplexAggregateRootTargetCreatedNewEvent ev)
            {
                Title = ev.Title;
                Quantity = ev.Quantity;
                ComplexAggRoot = this;
            }

            public override void InitializeEventHandlers()
            {
                Map<ComplexAggregateRootTargetCreatedNewEvent>().ToHandler(eventargs => NewTestComplexAggregateRootCreated(eventargs));
            }
        }

        [SetUp]
        public void Setup()
        {
            var service = new CommandService();
            service.RegisterExecutor(new AttributeMappedCommandExecutor<AggregateRootTargetUpdateTitleCommand>());
            service.RegisterExecutor(new AttributeMappedCommandExecutor<AggregateRootTargetCreateNewCommand>());
            service.RegisterExecutor(new AttributeMappedCommandExecutor<ComplexAggregateRootTargetCreateNewCommand1>());
            service.RegisterExecutor(new AttributeMappedCommandExecutor<ComplexAggregateRootTargetCreateNewCommand2>());
            service.RegisterExecutor(new AttributeMappedCommandExecutor<ComplexAggregateRootTargetCreateNewCommand3>());
            service.RegisterExecutor(new AttributeMappedCommandExecutor<ComplexAggregateRootTargetCreateNewCommand4>());

            TheService = service;
        }

        [Test]
        public void Command_should_update_the_title_of_the_aggregate_root()
        {
            var command = new AggregateRootTargetUpdateTitleCommand { Title = "AggregateRootTargetUpdateTitleCommand" };
            TheService.Execute(command);

            AggRoot.Title.Should().Be("AggregateRootTargetUpdateTitleCommand");
        }

        [Test]
        public void Command_should_throw_an_exception_when_the_command_is_not_mapped()
        {
            var command = new AggregateRootTargetNotAMappedCommand { Title = "AggregateRootTargetNotAMappedCommand" };

            Action act = () => TheService.Execute(command);
            act.ShouldThrow<CommandExecutorNotFoundException>();
        }

        [Test]
        public void Command_should_create_new_aggregate_root()
        {
            var command = new AggregateRootTargetCreateNewCommand { Title = "AggregateRootTargetCreateNewCommand" };
            TheService.Execute(command);

            AggRoot.Title.Should().Be("AggregateRootTargetCreateNewCommand");
        }

        [Test]
        public void Command_should_create_new_complex_aggregate_root_using_ordinal_parameter_mappings()
        {
            var command = new ComplexAggregateRootTargetCreateNewCommand1 {Title = "ComplexAggregateRootTargetCreateNewCommand1", Quantity = 10};
            TheService.Execute(command);

            ComplexAggRoot.Title.Should().Be("ComplexAggregateRootTargetCreateNewCommand1");
            ComplexAggRoot.Quantity.Should().Be(10);
        }

        [Test]
        public void Command_should_create_new_complex_aggregate_root_using_name_parameter_mappings()
        {
            var command = new ComplexAggregateRootTargetCreateNewCommand2 { Title = "ComplexAggregateRootTargetCreateNewCommand2", Quantity = 20 };
            TheService.Execute(command);

            ComplexAggRoot.Title.Should().Be("ComplexAggregateRootTargetCreateNewCommand2");
            ComplexAggRoot.Quantity.Should().Be(20);
        }

        [Test]
        public void Command_should_create_new_complex_aggregate_root_using_mixed_parameter_mappings()
        {
            var command = new ComplexAggregateRootTargetCreateNewCommand3 { Title = "ComplexAggregateRootTargetCreateNewCommand3", Quantity = 30 };
            TheService.Execute(command);

            ComplexAggRoot.Title.Should().Be("ComplexAggregateRootTargetCreateNewCommand3");
            ComplexAggRoot.Quantity.Should().Be(30);
        }

        [Test]
        public void Command_should_create_new_complex_aggregate_root_using_implicit_parameter_mappings()
        {
            var command = new ComplexAggregateRootTargetCreateNewCommand4 { Title = "ComplexAggregateRootTargetCreateNewCommand4", Quantity = 40 };
            TheService.Execute(command);

            ComplexAggRoot.Title.Should().Be("ComplexAggregateRootTargetCreateNewCommand4");
            ComplexAggRoot.Quantity.Should().Be(40);
        }
    }
}
