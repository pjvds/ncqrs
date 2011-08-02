using System;
using System.Transactions;
using FluentAssertions;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Domain;
using NUnit.Framework;

namespace Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes
{
    [TestFixture]
    public class AttributeCommandMappingTests
    {
        [MapsToAggregateRootMethod("Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes.AttributeCommandMappingTests+AggregateRootTarget, Ncqrs.Tests", "UpdateTitle")]
        public class AggregateRootTargetUpdateTitleCommand : CommandBase
        {
            public string Title
            { get; set; }

            [AggregateRootId]
            public Guid Id
            { get; set; }
        }

        [MapsToAggregateRootMethodOrConstructor("Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes.AttributeCommandMappingTests+AggregateRootTarget, Ncqrs.Tests", "UpdateTitle")]
        public class AggregateRootTargetCreateOrUpdateTitleCommand : CommandBase
        {
            public string Title
            { get; set; }

            [AggregateRootId]
            public Guid Id
            { get; set; }
        }

        [Transactional]
        [MapsToAggregateRootMethod("Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes.AttributeCommandMappingTests+AggregateRootTarget, Ncqrs.Tests", "UpdateTitle")]
        public class TransactionalAggregateRootTargetUpdateTitleCommand : CommandBase
        {
            public string Title
            { get; set; }

            [AggregateRootId]
            public Guid Id
            { get; set; }
        }

        [Transactional]
        [MapsToAggregateRootMethodOrConstructor("Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes.AttributeCommandMappingTests+AggregateRootTarget, Ncqrs.Tests", "UpdateTitle")]
        public class TransactionalAggregateRootTargetCreateOrUpdateTitleCommand : CommandBase
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

        [Transactional]
        [MapsToAggregateRootConstructor("Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes.AttributeCommandMappingTests+AggregateRootTarget, Ncqrs.Tests")]
        public class TransactionalAggregateRootTargetCreateNewCommand : CommandBase
        {
            public string Title
            { get; set; }

            [ExcludeInMapping]
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
            }

            private void NewTestAggregateRootCreated(AggregateRootTargetCreatedNewEvent ev)
            {
                Title = ev.Title;
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

        public class ComplexAggregateRootTargetCreatedNewEvent
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
            }

            public override void InitializeEventHandlers()
            {
                Map<ComplexAggregateRootTargetCreatedNewEvent>().ToHandler(NewTestComplexAggregateRootCreated);
            }
        }

        [Test]
        public void Command_should_update_the_title_of_the_aggregate_root()
        {
            var instance = new AggregateRootTarget("TitleSetInConstructor");
            var command = new AggregateRootTargetUpdateTitleCommand { Title = "AggregateRootTargetUpdateTitleCommand" };
            var executor = new TestAttributeMappedCommandExecutor<AggregateRootTarget>(command, instance);

            executor.Execute();

            executor.Instance.Title.Should().Be("AggregateRootTargetUpdateTitleCommand");
        }

        [Test]
        public void Command_should_create_and_update_the_title_of_the_aggregate_root()
        {
            AggregateRootTarget instance = null;
            var command = new AggregateRootTargetCreateOrUpdateTitleCommand { Title = "AggregateRootTargetUpdateTitleCommand" };
            var executor = new TestAttributeMappedCommandExecutor<AggregateRootTarget>(command, instance);

            executor.Execute();

            executor.Instance.Title.Should().Be("AggregateRootTargetUpdateTitleCommand");
        }

        [Test]
        public void Command_should_update_the_title_of_the_existing_aggregate_root()
        {
            var instance = new AggregateRootTarget("TitleSetInConstructor");
            var command = new AggregateRootTargetCreateOrUpdateTitleCommand { Title = "AggregateRootTargetUpdateTitleCommand" };
            var executor = new TestAttributeMappedCommandExecutor<AggregateRootTarget>(command, instance);

            executor.Execute();

            executor.Instance.Title.Should().Be("AggregateRootTargetUpdateTitleCommand");
        }

        [Test]
        public void Command_decorated_with_Transactional_attribute_mapped_to_methodOrCreator_should_be_create_and_be_executed_in_context_of_transaction()
        {
            bool executedInTransaction = false;
            AggregateRootTarget instance = null;
            var command = new TransactionalAggregateRootTargetCreateOrUpdateTitleCommand { Title = "TransactionalAggregateRootTargetUpdateTitleCommand" };
            var executor = new TestAttributeMappedCommandExecutor<AggregateRootTarget>(command, instance);
            executor.VerificationAction = () => executedInTransaction = Transaction.Current != null;

            executor.Execute();

            Assert.IsTrue(executedInTransaction);
        }

        [Test]
        public void Command_decorated_with_Transactional_attribute_mapped_to_methodOrCreator_should_be_executed_in_context_of_transaction()
        {
            bool executedInTransaction = false;
            var instance = new AggregateRootTarget("TitleSetInConstructor");
            var command = new TransactionalAggregateRootTargetCreateOrUpdateTitleCommand { Title = "TransactionalAggregateRootTargetUpdateTitleCommand" };
            var executor = new TestAttributeMappedCommandExecutor<AggregateRootTarget>(command, instance);
            executor.VerificationAction = () => executedInTransaction = Transaction.Current != null;

            executor.Execute();

            Assert.IsTrue(executedInTransaction);
        }

        [Test]
        public void Command_decorated_with_Transactional_attribute_mapped_to_method_should_be_executed_in_context_of_transaction()
        {
            bool executedInTransaction = false;
            var instance = new AggregateRootTarget("TitleSetInConstructor");
            var command = new TransactionalAggregateRootTargetUpdateTitleCommand { Title = "TransactionalAggregateRootTargetUpdateTitleCommand" };
            var executor = new TestAttributeMappedCommandExecutor<AggregateRootTarget>(command, instance);
            executor.VerificationAction = () => executedInTransaction = Transaction.Current != null;

            executor.Execute();

            Assert.IsTrue(executedInTransaction);
        }

        [Test]
        public void Command_decorated_with_Transactional_attribute_mapped_to_constructor_should_be_executed_in_context_of_transaction()
        {
            bool executedInTransaction = false;
            var command = new TransactionalAggregateRootTargetCreateNewCommand { Title = "TransactionalAggregateRootTargetCreateNewCommand" };
            var executor = new TestAttributeMappedCommandExecutor<AggregateRootTarget>(command);
            executor.VerificationAction = () => executedInTransaction = Transaction.Current != null;

            executor.Execute();

            Assert.IsTrue(executedInTransaction);
        }

        [Test]
        public void Command_should_throw_an_exception_when_the_command_is_not_mapped()
        {
            var command = new AggregateRootTargetNotAMappedCommand { Title = "AggregateRootTargetNotAMappedCommand" };
            var executor = new TestAttributeMappedCommandExecutor<AggregateRoot>(command);

            Action act = executor.Execute;
            act.ShouldThrow<CommandMappingException>();
        }

        [Test]
        public void Command_should_create_new_aggregate_root()
        {
            var command = new AggregateRootTargetCreateNewCommand { Title = "AggregateRootTargetCreateNewCommand" };
            var executor = new TestAttributeMappedCommandExecutor<AggregateRootTarget>(command);

            executor.Execute();

            executor.Instance.Title.Should().Be("AggregateRootTargetCreateNewCommand");
        }

        [Test]
        public void Command_should_create_new_complex_aggregate_root_using_ordinal_parameter_mappings()
        {
            var command = new ComplexAggregateRootTargetCreateNewCommand1 { Title = "ComplexAggregateRootTargetCreateNewCommand1", Quantity = 10 };
            var executor = new TestAttributeMappedCommandExecutor<ComplexAggregateRootTarget>(command);

            executor.Execute();

            executor.Instance.Title.Should().Be("ComplexAggregateRootTargetCreateNewCommand1");
            executor.Instance.Quantity.Should().Be(10);
        }

        [Test]
        public void Command_should_create_new_complex_aggregate_root_using_name_parameter_mappings()
        {
            var command = new ComplexAggregateRootTargetCreateNewCommand2 { Title = "ComplexAggregateRootTargetCreateNewCommand2", Quantity = 20 };
            var executor = new TestAttributeMappedCommandExecutor<ComplexAggregateRootTarget>(command);

            executor.Execute();

            executor.Instance.Title.Should().Be("ComplexAggregateRootTargetCreateNewCommand2");
            executor.Instance.Quantity.Should().Be(20);
        }

        [Test]
        public void Command_should_create_new_complex_aggregate_root_using_mixed_parameter_mappings()
        {
            var command = new ComplexAggregateRootTargetCreateNewCommand3 { Title = "ComplexAggregateRootTargetCreateNewCommand3", Quantity = 30 };
            var executor = new TestAttributeMappedCommandExecutor<ComplexAggregateRootTarget>(command);

            executor.Execute();

            executor.Instance.Title.Should().Be("ComplexAggregateRootTargetCreateNewCommand3");
            executor.Instance.Quantity.Should().Be(30);
        }

        [Test]
        public void Command_should_create_new_complex_aggregate_root_using_implicit_parameter_mappings()
        {
            var command = new ComplexAggregateRootTargetCreateNewCommand4 { Title = "ComplexAggregateRootTargetCreateNewCommand4", Quantity = 40 };
            var executor = new TestAttributeMappedCommandExecutor<ComplexAggregateRootTarget>(command);

            executor.Execute();

            executor.Instance.Title.Should().Be("ComplexAggregateRootTargetCreateNewCommand4");
            executor.Instance.Quantity.Should().Be(40);
        }
    }
}