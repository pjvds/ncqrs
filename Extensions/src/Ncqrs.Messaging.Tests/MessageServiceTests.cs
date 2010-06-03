using System;
using Ncqrs.Domain;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Storage;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Messaging.Tests
{
   [TestFixture]
   public class MessageServiceTests
   {
      private MessageService _sut;
      private IEventStore _eventStore;
      private IReceiverResolutionStrategy _receiverResolutionStrategy;
      private Guid _aggregateRootId;

      [SetUp]
      public void SetUp()
      {
         _aggregateRootId = Guid.NewGuid();
         _eventStore = MockRepository.GenerateMock<IEventStore>();
         NcqrsEnvironment.SetDefault(_eventStore);
         _receiverResolutionStrategy = MockRepository.GenerateMock<IReceiverResolutionStrategy>();
         _sut = new MessageService(_receiverResolutionStrategy);
      }      

      [Test]
      public void If_message_requires_receiver_existance_and_receiver_doesnt_exist_exception_is_thrown()
      {
         ExpectNotToFindExistingAggregate();         

         var message = new TestMessage
                          {                             
                             ProcessingRequirements = MessageProcessingRequirements.RequiresExisting
                          };
         Assert.Throws<MessageProcessingRequirementsViolationException>(() => _sut.Process(message));
      }

      

      [Test]
      public void If_message_requires_receiver_absence_and_receiver_exists_exception_is_thrown()
      {
         ExpectToFindExistingAggregate();

         var message = new TestMessage
         {            
            ProcessingRequirements = MessageProcessingRequirements.RequiresNew
         };
         Assert.Throws<MessageProcessingRequirementsViolationException>(() => _sut.Process(message));
      }

      [Test]
      public void If_receiver_doesnt_exist_new_one_is_created_before_processing_message()
      {
         ExpectNotToFindExistingAggregate();

         _sut.Process(new TestMessage());
      }

      [Test]
      public void If_receiver_exists_it_is_used_during_processing_of_a_message()
      {
         ExpectToFindExistingAggregate();

         _sut.Process(new TestMessage());
      }

      [Test]
      public void If_receiver_does_not_handle_message_type_exception_is_thrown()
      {
         ExpectToFindExistingInvalidAggregate();         

         var message = new TestMessage();
         Assert.Throws<MessageMappingException>(() => _sut.Process(message));
      }

      private void ExpectToFindExistingAggregate()
      {
         ResolveValidReceiver();
         _eventStore.Expect(x => x.GetAllEvents(Guid.Empty))
            .Return(new ISourcedEvent[] { new TestEvent(Guid.NewGuid(), _aggregateRootId, 1, DateTime.Now) })
            .IgnoreArguments()
            .Repeat.Any();
      }

      private void ExpectToFindExistingInvalidAggregate()
      {
         ResolveInvalidReceiver();
         _eventStore.Expect(x => x.GetAllEvents(Guid.Empty))
            .Return(new ISourcedEvent[] { new TestEvent(Guid.NewGuid(), _aggregateRootId, 1, DateTime.Now) })
            .IgnoreArguments()
            .Repeat.Any();
      }

      private void ExpectNotToFindExistingAggregate()
      {
         ResolveValidReceiver();
         _eventStore.Expect(x => x.GetAllEvents(Guid.Empty))
            .Return(new ISourcedEvent[] { })
            .IgnoreArguments()
            .Repeat.Any();
      }

      private void ResolveValidReceiver()
      {
         _receiverResolutionStrategy.Expect(x => x.Resolve(null))
            .IgnoreArguments()
            .Return(new ReceiverInfo(_aggregateRootId, typeof(Receiver)))
            .Repeat.Any();
      }

      private void ResolveInvalidReceiver()
      {
         _receiverResolutionStrategy.Expect(x => x.Resolve(null))
            .IgnoreArguments()
            .Return(new ReceiverInfo(_aggregateRootId, typeof(InvalidReceiver)))
            .Repeat.Any();
      }

      public class Receiver : MessagingAggregateRoot, IMessageHandler<TestMessage>
      {
         void IMessageHandler<TestMessage>.Handle(TestMessage testMessage)
         {                        
            Reply(new TestMessage()).Requiring(MessageProcessingRequirements.None);
         }

         private void OnTestEvent(TestEvent @event)
         {            
         }
      }

      public class InvalidReceiver : MessagingAggregateRoot
      {
         private void OnTestEvent(TestEvent @event)
         {
         }
      }

      public class TestMessage : MessageBase
      {
         public TestMessage()
         {
            MessageId = Guid.NewGuid();
            ReceiverId = "";
            SenderId = "";
         }
      }

      public class TestEvent : DomainEvent
      {
         public TestEvent(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp)
            : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
         {

         }
      }
   }
}