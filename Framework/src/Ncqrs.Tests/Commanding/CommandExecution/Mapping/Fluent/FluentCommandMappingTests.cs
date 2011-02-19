using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.Commanding.CommandExecution.Mapping.Fluent;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;
using FluentAssertions;

namespace Ncqrs.Tests.Commanding.CommandExecution.Mapping.Fluent
{
    [TestFixture]
    public class FluentCommandMappingTests
    {
        private ICommandService TheService
        { get; set; }

        public static AggregateRootTarget AggRoot
        { get; set; }

        private static AggregateRootTarget GetAggregateRoot()
        {
            return AggRoot ?? (AggRoot = new AggregateRootTarget("from GetAggregateRoot()"));
        }

        public class AggregateRootTargetUpdateTitleCommand : CommandBase
        {
            public string Title
            { get; set; }

            public Guid Id
            { get; set; }
        }

        public class AggregateRootTargetNotAMappedCommand : CommandBase
        {
            public string Title
            { get; set; }

            public Guid Id
            { get; set; }
        }

        public class AggregateRootTargetCreateNewCommand : CommandBase
        {
            public string Title
            { get; set; }

            public Guid Id
            { get; set; }
        }

        public class AggregateRootTargetTitleUpdatedEvent
        {
            public string Title
            { get; set; }
        }

        public class AggregateRootTargetCreatedNewEvent
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

            Map.Command<AggregateRootTargetUpdateTitleCommand>().ToAggregateRoot<AggregateRootTarget>().WithId(cmd => cmd.Id, (guid, knownVersion) => GetAggregateRoot()).ToCallOn((cmd, aggroot) => aggroot.UpdateTitle(cmd.Title)).RegisterWith(service);
            Map.Command<AggregateRootTargetCreateNewCommand>().ToAggregateRoot<AggregateRootTarget>().CreateNew((cmd) => new AggregateRootTarget(cmd.Title)).StoreIn((cmd, aggroot) => AggRoot = aggroot).RegisterWith(service);

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
            act.ShouldThrow<ExecutorForCommandNotFoundException>();
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
