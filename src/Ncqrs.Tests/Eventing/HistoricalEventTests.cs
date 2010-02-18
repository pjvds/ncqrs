using System;
using Moq;
using Ncqrs.Eventing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ncqs.EventingTests
{
    [TestClass]
    public class HistoricalEventTest
    {
        [TestMethod]
        public void PropertiesShouldBeInitializedWithTheGivenValuesAtConstruct()
        {
            var timeStamp = DateTime.Now;
            var evnt = new Mock<IEvent>().Object;

            var newHistoricalEvent = new HistoricalEvent(timeStamp, evnt);

            Assert.AreEqual(newHistoricalEvent.Event, evnt);
            Assert.AreEqual(newHistoricalEvent.TimeStamp, timeStamp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructingWithNullEventShouldThrowException()
        {
            var timeStamp = DateTime.Now;
            const IEvent nullEvent = null;

            new HistoricalEvent(timeStamp, nullEvent);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructingWithATimeStampOfDateTimeMinValueShoudThrowException()
        {
            IEvent nullEvent = new Mock<IEvent>().Object;

            new HistoricalEvent(DateTime.MinValue, nullEvent);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructingWithATimeStampOfDateTimeMaxValueShoudThrowException()
        {
            IEvent nullEvent = new Mock<IEvent>().Object;

            new HistoricalEvent(DateTime.MaxValue, nullEvent);
        }
    }
}
