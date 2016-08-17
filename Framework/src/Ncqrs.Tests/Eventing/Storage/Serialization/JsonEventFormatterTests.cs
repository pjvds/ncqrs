using System;
using FluentAssertions;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.Serialization;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Ncqrs.Tests.Eventing.Storage.Serialization
{
    public class JsonEventFormatterTests
    {
        private IEventTypeResolver _typeResolver;

        public JsonEventFormatterTests()
        {
            var typeResolver = new AttributeEventTypeResolver();
            typeResolver.AddEvent(typeof(AnEvent));
            _typeResolver = typeResolver;
        }

        [Fact]
        public void Ctor()
        {
            var formatter = new JsonEventFormatter(_typeResolver);
        }

        [Fact]
        public void Ctor_typeResolver_null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new JsonEventFormatter(null));
            ex.ParamName.Should().Be("typeResolver");
        }

        [Fact]
        public void Serialize()
        {
            var formatter = new JsonEventFormatter(_typeResolver);
            var theEvent = new AnEvent {
                Day = new DateTime(2000, 01, 01),
                Name = "Alice",
                Value = 10,
            };

            string eventName;
            var result = formatter.Serialize(theEvent, out eventName);

            eventName.Should().Be("bob");

            result.Should().NotBeNull();
            result.Count.Should().Be(3);
            result.Value<string>("Name").Should().Be(theEvent.Name);
            result.Value<int>("Value").Should().Be(theEvent.Value);
            result.Value<DateTime>("Day").Should().Be(theEvent.Day);
        }

        [Fact]
        public void Deserialize()
        {
            var formatter = new JsonEventFormatter(_typeResolver);
            var obj = new JObject(
                new JProperty("Name", "Alice"),
                new JProperty("Value", 10),
                new JProperty("Day", new DateTime(2000, 01, 01))
                );            

            var rawResult = formatter.Deserialize(obj, "bob");
            rawResult.Should().BeOfType<AnEvent>();

            var result = (AnEvent) rawResult;
            
            result.Name.Should().Be("Alice");
            result.Value.Should().Be(10);
            result.Day.Should().Be(new DateTime(2000, 01, 01));
        }

        [EventName("bob")]
        public class AnEvent
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public DateTime Day { get; set; }
        }
    }
}
