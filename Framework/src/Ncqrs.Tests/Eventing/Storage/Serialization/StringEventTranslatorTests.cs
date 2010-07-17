using System;
using FluentAssertions;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.Serialization;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing.Storage.Serialization
{
    [TestFixture]
    public class StringEventTranslatorTests
    {
        private StringEventTranslator _translator;

        [SetUp]
        public void Setup()
        {
            _translator = new StringEventTranslator();
        }

        [Test]
        public void TranslateToCommon()
        {
            var obj = CreateEvent(new JObject(
                new JProperty("Name", "Alice"),
                new JProperty("Value", 10)));

            var result = _translator.TranslateToRaw(obj);
            result.EventIdentifier.Should().Be(obj.EventIdentifier);
            result.EventSourceId.Should().Be(obj.EventSourceId);
            result.EventSequence.Should().Be(obj.EventSequence);
            result.EventTimeStamp.Should().Be(obj.EventTimeStamp);
            result.EventVersion.Should().Be(obj.EventVersion);
            result.Data.Should().Be("{\"Name\":\"Alice\",\"Value\":10}");

        }

        [Test]
        public void TranslateToCommon_obj_null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _translator.TranslateToCommon(null));
            ex.ParamName.Should().Be("obj");
        }

        [Test]
        public void TranslateToRaw()
        {
            var obj = CreateEvent("{\"Name\":\"Alice\",\"Value\":10}");

            var result = _translator.TranslateToCommon(obj);

            result.EventIdentifier.Should().Be(obj.EventIdentifier);
            result.EventSourceId.Should().Be(obj.EventSourceId);
            result.EventSequence.Should().Be(obj.EventSequence);
            result.EventTimeStamp.Should().Be(obj.EventTimeStamp);
            result.EventVersion.Should().Be(obj.EventVersion);
            result.Data.Should().NotBeNull();
        }

        [Test]
        public void TranslateToRaw_obj_null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _translator.TranslateToRaw(null));
            ex.ParamName.Should().Be("obj");
        }


        [Test]
        public void Dates_use_iso_format()
        {
            var obj = CreateEvent(new JObject(new JProperty("Value", new DateTime(2000, 01, 02, 03, 04, 05, 006))));

            var result = _translator.TranslateToRaw(obj);

            result.Data.Should().Be("{\"Value\":\"2000-01-02T03:04:05.006\"}");
        }

        [Test]
        public void Dates_respect_timezone()
        {
            var zone = new TimeSpan(0, 4, 0, 0);
            var obj = CreateEvent(new JObject(new JProperty("Value", new DateTimeOffset(2000, 01, 02, 03, 04, 05, 006, zone))));

            var result = _translator.TranslateToRaw(obj);

            result.Data.Should().Be("{\"Value\":\"2000-01-02T03:04:05.006+04:00\"}");
        }


        private StoredEvent<T> CreateEvent<T>(T data)
        {
            return new StoredEvent<T>(
                eventName: "bob",
                eventVersion: new Version(3, 0) /* different on purpose */,
                eventIdentifier: new Guid("402639D5-4106-4AE7-B210-45780C7A08F3"),
                eventSourceId: new Guid("697BAB3C-E122-4C70-85B0-76750BC2D878"),
                eventSequence: 1,
                eventTimeStamp: new DateTime(2001, 4, 12, 12, 52, 21, 425, DateTimeKind.Utc),
                data: data
            );
        }
    }
}
