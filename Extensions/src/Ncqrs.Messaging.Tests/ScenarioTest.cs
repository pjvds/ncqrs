using System;
using Ncqrs.Config;
using Ncqrs.Domain;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Spec;
using NUnit.Framework;
using Ncqrs.Eventing.Sourcing;
using System.Threading;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Ncqrs.Messaging.Tests
{
    [TestFixture]
    public class ScenarioTest
    {
        [Test]
        public void New_cargo_handling_event_is_registrered()
        {
            var cargoId = Guid.NewGuid();
            var firstEventId = Guid.NewGuid();
            var messageService = new MessageService();
            //messageService.UseReceivingStrategy(
            //    new ConditionalReceivingStrategy(
            //        x => x.GetType() == typeof(BookCargoMessage),
            //        new MappingReceivingStrategy<BookCargoMessage>(
            //            x => new IncomingMessage()
            //            {
            //                MessageId = x.MessageId,
            //                Payload = x,
            //                ProcessingRequirements = MessageProcessingRequirements.RequiresNew,
            //                ReceiverId = x.CargoId,
            //                ReceiverType = typeof(Cargo),
            //                SenderId = "Client"
            //            })));

            //messageService
            //    .ForIncomingMessage<RegisterHandlingEventTransportMesasge>()
            //    .AsPayloadUse(x => x)
            //    .AsMessageIdUse(x => x.MessageId)
            //    .AsReceiverIdUse(x => x.EventId)
            //    .AsSenderUse("Client");            

            //messageService
            //    .Map<RegisterHandlingEventTransportMesasge>().To<RegisterHandlingEventMessage>();                            

            //messageService.UseReceivingStrategy(
            //    new ConditionalReceivingStrategy(
            //        x => x.GetType() == typeof(RegisterHandlingEventMesasge),
            //        new MappingReceivingStrategy<RegisterHandlingEventMesasge>(
            //            x => new IncomingMessage()
            //            {
            //                MessageId = x.MessageId,
            //                Payload = x,
            //                ProcessingRequirements = MessageProcessingRequirements.RequiresExisting,
            //                ReceiverId = x.EventId,
            //                ReceiverType = typeof(HandlingEvent),
            //                SenderId = "Client"
            //            })));
            messageService.UseReceivingStrategy(new ConditionalReceivingStrategy(x => true, new LocalReceivingStrategy()));

            var messageSendingEventHandler = new MessageSendingEventHandler();
            var sendingStrategy = new FakeSendingStrategy(messageService);
            messageSendingEventHandler.UseStrategy(new ConditionalSendingStrategy(x => true, sendingStrategy));

            ((InProcessEventBus)NcqrsEnvironment.Get<IEventBus>()).RegisterHandler(messageSendingEventHandler);

            CommandService service = new CommandService();
            service.RegisterExecutorsInAssembly(this.GetType().Assembly);

            //Book new cargo
            //messageService.Process(new BookCargoMessage
            //{
            //    CargoId = cargoId,
            //    MessageId = Guid.NewGuid(),
            //});

            service.Execute(new BeginHandlingCommand() { Id = Guid.NewGuid(), cargoId = cargoId });

            //Register new handling event
            //messageService.Process(new RegisterHandlingEventMesasge
            //                          {
            //                              EventId = firstEventId,
            //                              MessageId = Guid.NewGuid(),
            //                              CargoId = cargoId
            //                          });

            //Process message from event to cargo
            //object message = sendingStrategy.DequeueMessage();
            //messageService.Process(message);

            //Thread.Sleep(1000); // The FakeSendingStrategy switches threads to process the message, so we need to wait for it to complete.

            //using (var uow = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork(Guid.NewGuid()))
            //{
            //    var cargo = (Cargo)uow.GetById(typeof(Cargo), cargoId, null);
            //    Assert.AreEqual(1, cargo.HandlingEventCount);
            //}
        }


        public class CommandExecutionTests : BigBangTestFixture<BeginHandlingCommand>
        {
            private readonly Guid cargoId = Guid.NewGuid();

            public CommandExecutionTests()
            {
               MessagingEnvironmentConfiguration.Configure();
            }

            protected override BeginHandlingCommand WhenExecuting()
            {
                return new BeginHandlingCommand
                {
                    Id = Guid.NewGuid(),
                    cargoId = cargoId
                };
            }

            [Test]
            public void it_should_not_throw()
            {
                var exception = CaughtException;
                Assert.IsNull(exception);
            }
        }

        public class MessagingEnvironmentConfiguration : IEnvironmentConfiguration
        {
            public static void Configure()
            {
                if (NcqrsEnvironment.IsConfigured) return;
                var cfg = new MessagingEnvironmentConfiguration();
                NcqrsEnvironment.Configure(cfg);
            }

            private static ICommandService InitializeCommandService()
            {
                var service = new CommandService();

                var testAssembly = typeof(ScenarioTest).Assembly;
                service.RegisterExecutorsInAssembly(testAssembly);

                return service;
            }

            private static IMessageService InitializeMessageService()
            {
                var messagingService = new MessageService();

                //messagingService.UseReceivingStrategy(IssueReferralMessageStrategy.ReceivingStrategy());
                messagingService.UseReceivingStrategy(new ConditionalReceivingStrategy(x => true, new LocalReceivingStrategy()));

                var messageSendingEventHandler = new MessageSendingEventHandler();
                var sendingStrategy = new FakeSendingStrategy(messagingService);
                messageSendingEventHandler.UseStrategy(new ConditionalSendingStrategy(x => true, sendingStrategy));

                var eventBus = ((InProcessEventBus)NcqrsEnvironment.Get<IEventBus>());
                eventBus.RegisterHandler(messageSendingEventHandler);

                return messagingService;
            }

            private readonly ICommandService commandService;
            private readonly IMessageService messageService;

            public MessagingEnvironmentConfiguration()
            {
                commandService = InitializeCommandService();
                messageService = InitializeMessageService();
            }
            public bool TryGet<T>(out T result) where T : class
            {
                result = null;
                if (typeof(T) == typeof(ICommandService))
                    result = (T)commandService;
                if (typeof(T) == typeof(IMessageService))
                    result = (T)messageService;
                return result != null;
            }
        }

        public class RegisterHandlingEventMesasge
        {
            public Guid MessageId { get; set; }
            public Guid EventId { get; set; }
            public Guid CargoId { get; set; }
        }

        [MapsToAggregateRootConstructor(typeof(HandlingEvent))]
        public class BeginHandlingCommand : CommandBase
        {
            public Guid Id { get; set; }
            public Guid cargoId { get; set; }
        }

        public class BookCargoMessage
        {
            public Guid MessageId { get; set; }
            public Guid CargoId { get; set; }
        }

        [MapsToAggregateRootConstructor(typeof(Cargo))]
        public class BookCargoCommand : CommandBase
        {
            public Guid Id { get; set; }
        }

        public class CargoWasHandledMessage
        {
        }

        public class HandlingBeganEvent { }

        public class HandlingEvent : MessagingAggregateRoot
        {
            private Guid _cargoId;

            public HandlingEvent()
            {
            }

            public HandlingEvent(Guid id, Guid cargoId)
                : base(id)
            {
                ApplyEvent(new HandlingEventRegistered
                {
                    CargoId = cargoId
                });

                To().Aggregate<Cargo>(_cargoId)
                    .Ensuring(MessageProcessingRequirements.RequiresNew)
                    .Send(new CargoWasHandledMessage());
            }

            private void OnHandlingEventRegistered(HandlingEventRegistered @event)
            {
                _cargoId = @event.CargoId;
            }
        }

        public class Cargo : MessagingAggregateRoot,
           IMessageHandler<BookCargoMessage>,
           IMessageHandler<CargoWasHandledMessage>
        {
            private int _handlingEventCount;

            public int HandlingEventCount
            {
                get { return _handlingEventCount; }
            }

            public Cargo(Guid id)
                : base(id)
            {
            }

            public Cargo()
                : base()
            {
            }

            public void Handle(BookCargoMessage message)
            {
                ApplyEvent(new CargoBooked());
            }

            private void OnCargoBooked(CargoBooked @event)
            {
            }

            public void Handle(CargoWasHandledMessage message)
            {
                ApplyEvent(new CargoHandled());
            }

            private void OnCargoHandled(CargoHandled @event)
            {
                _handlingEventCount++;
            }
        }

        public class HandlingEventRegistered
        {
            public Guid CargoId { get; set; }
        }

        public class CargoBooked
        {
        }

        public class CargoHandled
        {
        }
    }
}