using System;
using Ncqrs.Domain;
using Ncqrs.Eventing.ServiceModel.Bus;
using NUnit.Framework;

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
         var clientId = Guid.NewGuid();
         var messageService = new MessageService(new LocalResolutionStrategy());
         var messageSendingEventHandler = new MessageSendingEventHandler();
         var messageSender = new InMemoryLocalMessageSender();
         messageSendingEventHandler.AddSender(messageSender);
         NcqrsEnvironment.Get<IEventBus>().RegisterHandler(messageSendingEventHandler);

         //Book new cargo
         messageService.Process(new BookCargoMessage
                                   {
                                      ReceiverId = LocalResolutionStrategy.MakeId(typeof(Cargo),cargoId),
                                      MessageId = Guid.NewGuid(),
                                      ProcessingRequirements = MessageProcessingRequirements.RequiresNew,                                      
                                      SenderId = clientId.ToString()
                                   });

         //Register new handling event
         messageService.Process(new RegisterHandlingEventMesasge
                                   {
                                      ReceiverId = LocalResolutionStrategy.MakeId(typeof(HandlingEvent),firstEventId),
                                      MessageId = Guid.NewGuid(),
                                      ProcessingRequirements = MessageProcessingRequirements.RequiresNew,
                                      SenderId = clientId.ToString(),
                                      CargoId = cargoId
                                   });

         //Process message from event to cargo
         Assert.IsTrue(messageSender.ProcessNext());

         using (var uow = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
         {
            var cargo = uow.GetById<Cargo>(cargoId);
            Assert.AreEqual(1, cargo.HandlingEventCount);
         }
      }

      public class RegisterHandlingEventMesasge : MessageBase
      {
         public Guid CargoId { get; set; }
      }

      public class BookCargoMessage : MessageBase
      {
      }

      public class CargoWasHandledMessage : MessageBase
      {         
      }

      public class HandlingEvent : MessagingAggregateRoot, IMessageHandler<RegisterHandlingEventMesasge>
      {
         private Guid _cargoId;

         public void Handle(RegisterHandlingEventMesasge message)
         {
            ApplyEvent(new HandlingEventRegistered
                          {
                             Id = new LocalResolutionStrategy().Resolve(message.ReceiverId).Id,
                             CargoId = message.CargoId
                          });

            Send(new CargoWasHandledMessage())
               .To<Cargo>(_cargoId)
               .Requiring(MessageProcessingRequirements.RequiresExisting);            
         }

         private void OnHandlingEventRegistered(HandlingEventRegistered @event)
         {
            Id = @event.Id;
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

         public void Handle(BookCargoMessage message)
         {
            ApplyEvent(new CargoBooked
                          {
                             Id = new LocalResolutionStrategy().Resolve(message.ReceiverId).Id
                          });
         }

         private void OnCargoBooked(CargoBooked @event)
         {
            Id = @event.Id;
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

      public class HandlingEventRegistered : DomainEvent
      {
         public Guid Id { get; set; }
         public Guid CargoId { get; set; }
      }

      public class CargoBooked : DomainEvent
      {
         public Guid Id { get; set; }
      }

      public class CargoHandled : DomainEvent
      {
      }
   }
}