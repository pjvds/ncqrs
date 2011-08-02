using System;
using FluentAssertions;
using NUnit.Framework;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.CommandExecution.Mapping.Fluent;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Domain;

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

        private static AggregateRootTarget GetAggregateRoot(Guid ID)
        {
            if (AggRoot == null)
            {
                return new AggregateRootTarget("new Agg Root");
            }

            if (AggRoot.ArId == ID)
            {
                return AggRoot;
            }
            else
            {
                return new AggregateRootTarget("new and Different Agg Root");
            }
        }

        public class AggregateRootTargetUpdateTitleCommand : CommandBase
        {
            public string Title
            { get; set; }

            public Guid Id
            { get; set; }
        }

        public class AggregateRootTargetCreateOrUpdateTitleCommand : CommandBase
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

        public class AggregateRootTargetStaticCreateCommand : CommandBase
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

            public Guid ArId
            { get; private set; }

            public long created = DateTime.Now.Ticks;

            public AggregateRootTarget(string title)
            {
                ArId = Guid.NewGuid();
                var eventargs = new AggregateRootTargetCreatedNewEvent { Title = title };
                ApplyEvent(eventargs);
            }

            public AggregateRootTarget(Guid id, string title)
            {
                ArId = id;
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

            public static AggregateRootTarget CreateNew(string title)
            {
                return new AggregateRootTarget(title);
            }
        }

        [SetUp]
        public void Setup()
        {
            var service = new CommandService();

            Map.Command<AggregateRootTargetStaticCreateCommand>().ToAggregateRoot<AggregateRootTarget>().CreateNew((cmd) => AggregateRootTarget.CreateNew(cmd.Title)).StoreIn((cmd, aggroot) => AggRoot = aggroot).RegisterWith(service);
            Map.Command<AggregateRootTargetUpdateTitleCommand>().ToAggregateRoot<AggregateRootTarget>().WithId(cmd => cmd.Id, (guid, knownVersion) => GetAggregateRoot()).ToCallOn((cmd, aggroot) => aggroot.UpdateTitle(cmd.Title)).RegisterWith(service);

            Map.Command<AggregateRootTargetCreateNewCommand>().ToAggregateRoot<AggregateRootTarget>().CreateNew((cmd) => new AggregateRootTarget(cmd.Title)).StoreIn((cmd, aggroot) => AggRoot = aggroot).RegisterWith(service);

            Map.Command<AggregateRootTargetCreateOrUpdateTitleCommand>().ToAggregateRoot<AggregateRootTarget>().UseExistingOrCreateNew(cmd => cmd.Id, (guid, knownVersion) => GetAggregateRoot(guid), (cmd) => new AggregateRootTarget(cmd.Id, cmd.Title)).ToCallOn((cmd, aggroot) => aggroot.UpdateTitle(cmd.Title)).RegisterWith(service);

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

        [Test]
        public void Command_should_create_new_aggregate_root_with_static_method()
        {
            var command = new AggregateRootTargetStaticCreateCommand { Title = "AggregateRootTargetStaticCreateCommand" };
            TheService.Execute(command);

            AggRoot.Title.Should().Be("AggregateRootTargetStaticCreateCommand");
        }

        [Test]
        public void Command_should_create_and_then_use_existing()
        {
            var command = new AggregateRootTargetCreateOrUpdateTitleCommand { Title = "AggregateRootCreateNewCommand", Id = Guid.NewGuid() };
            TheService.Execute(command);

            AggRoot.Title.Should().Be("AggregateRootCreateNewCommand");
            var arId = AggRoot.ArId;

            command = new AggregateRootTargetCreateOrUpdateTitleCommand { Title = "AggregateRootCreateNewCommand2", Id = Guid.NewGuid() };
            TheService.Execute(command);

            AggRoot.Title.Should().Be("AggregateRootCreateNewCommand2");
            Assert.AreNotEqual(arId, AggRoot.ArId, "Id's should be different.");

            var createTicks = AggRoot.created;
            arId = AggRoot.ArId;

            command = new AggregateRootTargetCreateOrUpdateTitleCommand { Title = "AggregateRootUpdatedCommand", Id = arId };
            TheService.Execute(command);

            AggRoot.Title.Should().Be("AggregateRootUpdatedCommand");
            AggRoot.ArId.Should().Be(arId);
            AggRoot.created.Should().Be(createTicks);
        }
    }
}