using System;
using Ncqrs.Domain;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Storage;
using NUnit.Framework;
using Rhino.Mocks;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Messaging.Tests
{
    [TestFixture]
    public class MessageServiceTests
    {
        private MessageService _sut;
        private IEventStore _eventStore;
        private IReceivingStrategy _receivingStrategy;
        private Guid _aggregateRootId;

        [SetUp]
        public void SetUp()
        {
            _aggregateRootId = Guid.NewGuid();
            _eventStore = MockRepository.GenerateMock<IEventStore>();
            NcqrsEnvironment.SetDefault(_eventStore);
            _receivingStrategy = MockRepository.GenerateMock<IReceivingStrategy>();
            _sut = new MessageService();
            _sut.UseReceivingStrategy(new ConditionalReceivingStrategy(x => true, _receivingStrategy));
        }

        [Test]
        public void If_message_requires_receiver_existance_and_receiver_doesnt_exist_exception_is_thrown()
        {
            ExpectNotToFindExistingAggregate();
            ResolveValidReceiver(true);
            Assert.Throws<MessageProcessingRequirementsViolationException>(() => _sut.Process(new TestMessage()));
        }



        [Test]
        public void If_message_requires_receiver_absence_and_receiver_exists_exception_is_thrown()
        {
            ExpectToFindExistingAggregate();
            ResolveValidReceiver(false);
            Assert.Throws<MessageProcessingRequirementsViolationException>(() => _sut.Process(new TestMessage()));
        }

        [Test]
        public void If_receiver_doesnt_exist_new_one_is_created_before_processing_message()
        {
            ExpectNotToFindExistingAggregate();
            ResolveValidReceiver(false);
            _sut.Process(new TestMessage());
        }

        [Test]
        public void If_receiver_exists_it_is_used_during_processing_of_a_message()
        {
            ExpectToFindExistingAggregate();
            ResolveValidReceiver(true);
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
            _eventStore.Expect(x => x.ReadFrom(Guid.Empty, long.MinValue, long.MaxValue))
               .Return(new CommittedEventStream(_aggregateRootId, new[] { new CommittedEvent(Guid.NewGuid(), Guid.NewGuid(), _aggregateRootId, 1, DateTime.Now, new TestEvent(), new Version(1,0)) }))
               .IgnoreArguments()
               .Repeat.Any();
        }

        private void ExpectToFindExistingInvalidAggregate()
        {
            ResolveInvalidReceiver();
            _eventStore.Expect(x => x.ReadFrom(Guid.Empty, long.MinValue, long.MaxValue))
               .Return(new CommittedEventStream(_aggregateRootId, new[] { new CommittedEvent(Guid.NewGuid(), Guid.NewGuid(), _aggregateRootId, 1, DateTime.Now, new TestEvent(), new Version(1,0)) }))
               .IgnoreArguments()
               .Repeat.Any();
        }

        private void ExpectNotToFindExistingAggregate()
        {
            _eventStore.Expect(x => x.ReadFrom(Guid.Empty, long.MinValue, long.MaxValue))
               .Return(new CommittedEventStream(Guid.Empty))
               .IgnoreArguments()
               .Repeat.Any();
        }

        private void ResolveValidReceiver(bool requireExisting)
        {
            _receivingStrategy.Expect(x => x.Receive(null))
               .IgnoreArguments()
               .Return(new IncomingMessage
                           {
                               Payload = new TestMessage(),
                               ReceiverType = typeof(Receiver),
                               ReceiverId = _aggregateRootId,
                               ProcessingRequirements = requireExisting ? MessageProcessingRequirements.RequiresExisting : MessageProcessingRequirements.RequiresNew
                           })
               .Repeat.Any();
        }

        private void ResolveInvalidReceiver()
        {
            _receivingStrategy.Expect(x => x.Receive(null))
               .IgnoreArguments()
               .Return(new IncomingMessage
                           {
                               Payload = new TestMessage(),
                               ReceiverType = typeof(InvalidReceiver),
                               ReceiverId = _aggregateRootId
                           })
               .Repeat.Any();
        }

        public class Receiver : MessagingAggregateRoot, IMessageHandler<TestMessage>
        {
            void IMessageHandler<TestMessage>.Handle(TestMessage testMessage)
            {
                Reply(new TestMessage());
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

        public class TestMessage
        {            
        }

        public class TestEvent
        {            
        }
    }
}