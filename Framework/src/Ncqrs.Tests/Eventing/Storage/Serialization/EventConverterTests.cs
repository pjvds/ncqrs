using System;
using FluentAssertions;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.Serialization;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Tests.Eventing.Storage.Serialization
{
    [TestFixture]
    public class EventConverterTests
    {
        private IEventTypeResolver _typeResolver;
        private IEventConverter _childConverter = new NullEventConverter();
        private const string EventName = "bob";

        [SetUp]
        public void SetUp()
        {
            _typeResolver = MockRepository.GenerateStub<IEventTypeResolver>();
            _typeResolver
                .Stub(x => x.EventNameFor(typeof(AnEvent)))
                .TentativeReturn()
                .Return(EventName);

            _childConverter = MockRepository.GenerateStub<IEventConverter>();
        }

        [Test]
        public void Ctor()
        {
            Assert.DoesNotThrow(() => new EventConverter(_typeResolver));
        }

        [Test]
        public void Ctor_typeResolver_null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new EventConverter(null));
            ex.ParamName.Should().Be("typeResolver");
        }

        [Test]
        public void Upgrade()
        {
            var event1 = CreateEvent("foo");
            var event2 = CreateEvent("bar");
            var fooConverter = MockRepository.GenerateStub<IEventConverter>();
            var barConverter = MockRepository.GenerateStub<IEventConverter>();

            var converter = new EventConverter(_typeResolver);
            converter.AddConverter("foo", fooConverter);
            converter.AddConverter("bar", barConverter);

            converter.Upgrade(event1);
            converter.Upgrade(event2);

            fooConverter.AssertWasCalled(x => x.Upgrade(event1));
            barConverter.AssertWasCalled(x => x.Upgrade(event2));
        }

        [Test]
        public void Upgrade_unknown_event()
        {
            var converter = new EventConverter(_typeResolver);
            Assert.DoesNotThrow(() => converter.Upgrade(CreateEvent()));
        }

        [Test]
        public void Upgrade_theEvent_null()
        {
            var converter = new EventConverter(_typeResolver);
            var ex = Assert.Throws<ArgumentNullException>(() => converter.Upgrade(null));
            ex.ParamName.Should().Be("theEvent");
        }

        [Test]
        public void AddConverter_ByType()
        {
            var anEvent = CreateEvent();
            var converter = new EventConverter(_typeResolver);
            converter.AddConverter(typeof(AnEvent), _childConverter);

            converter.Upgrade(anEvent);

            _childConverter.AssertWasCalled(x => x.Upgrade(anEvent));
        }

        [Test]
        public void AddConverter_ByType_eventType_null()
        {
            var converter = new EventConverter(_typeResolver);
            var ex = Assert.Throws<ArgumentNullException>(() => converter.AddConverter((Type) null, _childConverter));
            ex.ParamName.Should().Be("eventType");
        }

        [Test]
        public void AddConverter_ByType_converter_null()
        {
            var converter = new EventConverter(_typeResolver);
            var ex = Assert.Throws<ArgumentNullException>(() => converter.AddConverter(typeof(AnEvent), null));
            ex.ParamName.Should().Be("converter");
        }

        [Test]
        public void AddConverter_ByName()
        {
            var anEvent = CreateEvent();
            var converter = new EventConverter(_typeResolver);
            converter.AddConverter(EventName, _childConverter);

            converter.Upgrade(anEvent);

            _childConverter.AssertWasCalled(x => x.Upgrade(anEvent));
        }

        [Test]
        public void AddConverter_ByName_eventName_null()
        {
            var converter = new EventConverter(_typeResolver);
            var ex = Assert.Throws<ArgumentNullException>(() => converter.AddConverter((string) null, _childConverter));
            ex.ParamName.Should().Be("eventName");
        }

        [Test]
        public void AddConverter_ByName_converter_null()
        {
            var converter = new EventConverter(_typeResolver);
            var ex = Assert.Throws<ArgumentNullException>(() => converter.AddConverter(EventName, null));
            ex.ParamName.Should().Be("converter");
        }

        [Test]
        public void AddConverter_duplicate_name()
        {
            var converter = new EventConverter(_typeResolver);
            converter.AddConverter(EventName, _childConverter);

            var ex = Assert.Throws<ArgumentException>(() => converter.AddConverter(EventName, _childConverter));
            ex.ParamName.Should().Be("eventName");
            ex.Message.Should().StartWith("There is already a converter for event 'bob'.");
        }


        private StoredEvent<JObject> CreateEvent()
        {
            return CreateEvent(EventName);
        }

        private StoredEvent<JObject> CreateEvent(string name)
        {
            var id = new Guid("36C84164-B324-496A-9098-1A5D9945DD88");
            var sourceId = new Guid("A710F584-7134-4BAB-9741-75A35F9C1E02");
            var theDate = new DateTime(2000, 01, 01);

            JObject obj = new JObject();
            return new StoredEvent<JObject>(id, theDate, name, new Version(1, 0), sourceId, 1, obj);
        }

        private class AnEvent {}
    }
}
