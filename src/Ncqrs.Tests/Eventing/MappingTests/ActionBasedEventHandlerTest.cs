// TODO: Move to event bus test project.
//using System;
//using Ncqrs.Eventing;
//using Ncqrs.Eventing.Mapping;
//using Moq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace Ncqs.EventingTests.MappingTests
//{
//    [TestClass]
//    public class ActionBasedEventHandlerTest
//    {
//        [TestMethod]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void ConstructingActionBasedEventHandlerWithANullActionShouldThrowException()
//        {
//            const Action<IEvent> nullAction = null;
//            new ActionBasedEventHandler(nullAction);
//        }

//        [TestMethod]
//        public void ConstructingObjectWithCorrectParameterValuesShouldNotThrowAnyExceptions()
//        {
//            var action = new Action<IEvent>((x) => x.GetType());
//            new ActionBasedEventHandler(action);
//        }

//        [TestMethod]
//        public void CallingInvokeShouldExecuteTheActionThatWasAssignedWhenConstructing()
//        {
//            bool actionIsInvoked = false;

//            var evnt = new Mock<IEvent>().Object;
//            var action = new Action<IEvent>((x) => actionIsInvoked = true);
//            var handler = new ActionBasedEventHandler(action);
//            handler.Invoke(evnt);

//            Assert.IsTrue(actionIsInvoked);
//        }

//        [TestMethod]
//        public void CallingInvokeWithASpecifiedEventShouldInvokeTheActionWithThatSameEventAsArgument()
//        {
//            bool eventWasEqual = false;

//            var evnt = new Mock<IEvent>().Object;
//            var action = new Action<IEvent>((x) => eventWasEqual = (x == evnt));
//            var handler = new ActionBasedEventHandler(action);
//            handler.Invoke(evnt);

//            Assert.IsTrue(eventWasEqual);
//        }
//    }
//}
