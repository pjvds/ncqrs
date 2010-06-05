using System;
using System.Linq;
using Ncqrs.Domain;
using NUnit.Framework;

namespace Ncqrs.Messaging.Tests
{
   [TestFixture]
   public class MessagingAggregateRootTests
   {
      [Test]
      public void After_sending_message_event_is_applied()
      {
         using (NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
         {
            IMessagingAggregateRoot root = new TestMessagingAggregateRoot();
            root.ProcessMessage(new IncomingMessage()
                                    {
                                        Payload = new TestMessage()
                                    });

            Assert.AreEqual(1, ((AggregateRoot)root).GetUncommittedEvents().Count());
         }
      }

      [Test]
      public void If_same_message_is_received_twice_only_the_first_one_is_processed()
      {
         using (NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
         {
            IMessagingAggregateRoot root = new TestMessagingAggregateRoot();

            var testMessage = new IncomingMessage()
                                  {
                                      Payload = new TestMessage()
                                  };
            root.ProcessMessage(testMessage);
            root.ProcessMessage(testMessage);

            Assert.AreEqual(1, ((AggregateRoot)root).GetUncommittedEvents().Count());
         }
      }

      public class TestMessagingAggregateRoot : MessagingAggregateRoot, IMessageHandler<TestMessage>
      {
         public void SendSomething()
         {
             To().Aggregate<TestMessagingAggregateRoot>(Guid.NewGuid())
                 .Ensuring(MessageProcessingRequirements.RequiresExisting)
                 .Send(new TestMessage());            
         }

         public void Handle(TestMessage message)
         {
         }
      }

      public class TestMessage : IncomingMessage
      {         
      }
   }
}