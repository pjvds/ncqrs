using System;
using FluentAssertions;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.Serialization;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing.Storage.Serialization
{
    [TestFixture]
    public class JsonEventFormatterTests
    {
        private IEventTypeResolver _typeResolver;

        [SetUp]
        public void SetUp()
        {
            var typeResolver = new AttributeEventTypeResolver();
            typeResolver.AddEvent(typeof(AnEvent));
            _typeResolver = typeResolver;
        }

        [Test]
        public void Ctor()
        {
            Assert.DoesNotThrow(() => new JsonEventFormatter(_typeResolver));
        }

        [Test]
        public void Ctor_typeResolver_null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new JsonEventFormatter(null));
            ex.ParamName.Should().Be("typeResolver");
        }

        [Test]
        public void Serialize()
        {
            Assert.Ignore("Throwing an error about not finding NUnit.dll - not sure why...");
            var formatter = new JsonEventFormatter(_typeResolver);
            var theEvent = new AnEvent(
                eventIdentifier: new Guid("402639D5-4106-4AE7-B210-45780C7A08F3"),
                eventSourceId: new Guid("697BAB3C-E122-4C70-85B0-76750BC2D878"),
                eventSequence: 1,
                eventTimeStamp: new DateTime(2001, 4, 12, 12, 52, 21, 425, DateTimeKind.Utc)) {
                Day = new DateTime(2000, 01, 01),
                Name = "Alice",
                Value = 10,
            };

            var result = formatter.Serialize(theEvent);

            result.EventName.Should().Be("bob");
            result.EventIdentifier.Should().Be(theEvent.EventIdentifier);
            result.EventSourceId.Should().Be(theEvent.EventSourceId);
            result.EventSequence.Should().Be(theEvent.EventSequence);
            result.EventTimeStamp.Should().Be(theEvent.EventTimeStamp);
            result.EventVersion.Should().Be(theEvent.EventVersion);

            result.Data.Should().NotBeNull();
            result.Data.Count.Should().Be(3);
            result.Data.Value<string>("Name").Should().Be(theEvent.Name);
            result.Data.Value<int>("Value").Should().Be(theEvent.Value);
            result.Data.Value<DateTime>("Day").Should().Be(theEvent.Day);
        }

        [Test]
        public void Serialize_theEvent_null()
        {
            var formatter = new JsonEventFormatter(_typeResolver);
            var ex = Assert.Throws<ArgumentNullException>(() => formatter.Serialize(null));
            ex.ParamName.Should().Be("theEvent");
        }

        [Test]
        public void Deserialize()
        {
            var formatter = new JsonEventFormatter(_typeResolver);
            var obj = new StoredEvent<JObject>(
                eventName: "bob",
                eventVersion: new Version(3, 0) /* different on purpose */,
                eventIdentifier: new Guid("402639D5-4106-4AE7-B210-45780C7A08F3"),
                eventSourceId: new Guid("697BAB3C-E122-4C70-85B0-76750BC2D878"),
                eventSequence: 1,
                eventTimeStamp: new DateTime(2001, 4, 12, 12, 52, 21, 425, DateTimeKind.Utc),
                data: new JObject(
                    new JProperty("Name", "Alice"),
                    new JProperty("Value", 10),
                    new JProperty("Day", new DateTime(2000, 01, 01))
                )
            );

            var rawResult = formatter.Deserialize(obj);
            rawResult.Should().BeOfType<AnEvent>();

            var result = (AnEvent) rawResult;
            result.EventIdentifier.Should().Be(obj.EventIdentifier);
            result.EventSourceId.Should().Be(obj.EventSourceId);
            result.EventSequence.Should().Be(obj.EventSequence);
            result.EventTimeStamp.Should().Be(obj.EventTimeStamp);
            result.EventVersion.Should().Be(new Version(2, 0));
            
            result.Name.Should().Be("Alice");
            result.Value.Should().Be(10);
            result.Day.Should().Be(new DateTime(2000, 01, 01));
        }

        [Test]
        public void Deserialize_obj_null()
        {
            var formatter = new JsonEventFormatter(_typeResolver);
            var ex = Assert.Throws<ArgumentNullException>(() => formatter.Deserialize(null));
            ex.ParamName.Should().Be("obj");
        }


        [EventName("bob")]
        public class AnEvent : SourcedEvent
        {
            public AnEvent()
            {
                EventVersion = new Version(2, 0);
            }

            public AnEvent(Guid eventIdentifier, Guid eventSourceId, long eventSequence, DateTime eventTimeStamp) : base(eventIdentifier, eventSourceId, eventSequence, eventTimeStamp)
            {
                EventVersion = new Version(2, 0);
            }

            public string Name { get; set; }
            public int Value { get; set; }
            public DateTime Day { get; set; }
        }
    }
}
