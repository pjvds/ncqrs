using System;
using Ncqrs.Domain;
using Ncqrs.Eventing.ServiceModel.Bus;
using NUnit.Framework;
using Ncqrs.Eventing.Sourcing;

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
            messageService.UseReceivingStrategy(
                new ConditionalReceivingStrategy(
                    x => x.GetType() == typeof (BookCargoMessage),
                    new MappingReceivingStrategy<BookCargoMessage>(
                        x => new IncomingMessage()
                                 {
                                     MessageId = x.MessageId,
                                     Payload = x,
                                     ProcessingRequirements = MessageProcessingRequirements.RequiresNew,
                                     ReceiverId = x.CargoId,
                                     ReceiverType = typeof (Cargo),
                                     SenderId = "Client"
                                 })));

            //messageService
            //    .ForIncomingMessage<RegisterHandlingEventTransportMesasge>()
            //    .AsPayloadUse(x => x)
            //    .AsMessageIdUse(x => x.MessageId)
            //    .AsReceiverIdUse(x => x.EventId)
            //    .AsSenderUse("Client");            

            //messageService
            //    .Map<RegisterHandlingEventTransportMesasge>().To<RegisterHandlingEventMessage>();                            

            messageService.UseReceivingStrategy(
                new ConditionalReceivingStrategy(
                    x => x.GetType() == typeof (RegisterHandlingEventMesasge),
                    new MappingReceivingStrategy<RegisterHandlingEventMesasge>(
                        x => new IncomingMessage()
                                 {
                                     MessageId = x.MessageId,
                                     Payload = x,
                                     ProcessingRequirements = MessageProcessingRequirements.RequiresNew,
                                     ReceiverId = x.EventId,
                                     ReceiverType = typeof (HandlingEvent),
                                     SenderId = "Client"
                                 })));
            messageService.UseReceivingStrategy(new ConditionalReceivingStrategy(x => true, new LocalReceivingStrategy()));

            var messageSendingEventHandler = new MessageSendingEventHandler();
            var sendingStrategy = new FakeSendingStrategy();
            messageSendingEventHandler.UseStrategy(new ConditionalSendingStrategy(x => true, sendingStrategy));
            ((InProcessEventBus)NcqrsEnvironment.Get<IEventBus>()).RegisterHandler(messageSendingEventHandler);

            //Book new cargo
            messageService.Process(new BookCargoMessage
                                      {
                                          CargoId = cargoId,
                                          MessageId = Guid.NewGuid(),                                          
                                      });

            //Register new handling event
            messageService.Process(new RegisterHandlingEventMesasge
                                      {
                                          EventId = firstEventId,
                                          MessageId = Guid.NewGuid(),
                                          CargoId = cargoId
                                      });

            //Process message from event to cargo
            object message = sendingStrategy.DequeueMessage();
            messageService.Process(message);

            using (var uow = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork(Guid.NewGuid()))
            {
                var cargo = (Cargo)uow.GetById(typeof(Cargo),cargoId, null);
                Assert.AreEqual(1, cargo.HandlingEventCount);
            }
        }

        public class RegisterHandlingEventMesasge
        {
            public Guid MessageId { get; set; }
            public Guid EventId { get; set; }
            public Guid CargoId { get; set; }
        }

        public class BookCargoMessage
        {
            public Guid MessageId { get; set; }
            public Guid CargoId { get; set; }
        }

        public class CargoWasHandledMessage
        {
        }

        public class HandlingEvent : MessagingAggregateRoot, IMessageHandler<RegisterHandlingEventMesasge>
        {
            private Guid _cargoId;

            public HandlingEvent()
            {                
            }

            public HandlingEvent(Guid id) : base(id)
            {                
            }

            public void Handle(RegisterHandlingEventMesasge message)
            {
                ApplyEvent(new HandlingEventRegistered
                              {
                                  CargoId = message.CargoId
                              });

                To().Aggregate<Cargo>(_cargoId)
                    .Ensuring(MessageProcessingRequirements.RequiresExisting)
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

            public Cargo(Guid id) : base(id)
            {                
            }

            public Cargo() : base()
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