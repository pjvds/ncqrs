using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.Commanding.CommandExecution.Mapping.Actions;
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

        [SetUp]
        public void Setup()
        {
            var service = new CommandService();

            Map.Command(new AttributeMappedCommandExecutor<AggregateRootTargetUpdateTitleCommand>()).RegisterWith(service);
            Map.Command(new AttributeMappedCommandExecutor<AggregateRootTargetCreateNewCommand>()).RegisterWith(service);

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
    }
}
