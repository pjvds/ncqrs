using System;
using FluentAssertions;
using Ncqrs.Eventing;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Tests.Eventing
{
    [TestFixture]
    public class EventBaseSpecs
    {
        [Test]
        public void Constructing_a_new_event_base_it_should_call_the_GenerateNewId_method_from_the_generator_that_has_been_set_in_the_environment()
        {
            var generator = MockRepository.GenerateMock<IUniqueIdentifierGenerator>();
            NcqrsEnvironment.SetDefault<IUniqueIdentifierGenerator>(generator);

            var mock = MockRepository.GenerateStub<Event>();

            generator.AssertWasCalled(g=>g.GenerateNewId());
        }

        [Test]
        public void Constructing_a_new_event_base_it_should_set_the_event_identifier_to_identifier_that_has_been_given_from_the_IUniqueIdentifierGenerator_from_the_NcqrsEnvironment()
        {
            var identiefier = Guid.NewGuid();

            var generator = MockRepository.GenerateStrictMock<IUniqueIdentifierGenerator>();
            generator.Stub(g => g.GenerateNewId()).Return(identiefier);

            NcqrsEnvironment.SetDefault<IUniqueIdentifierGenerator>(generator);

            var mock = MockRepository.GenerateStub<Event>();
            mock.EventIdentifier.Should().Be(identiefier);
        }

        [Test]
        public void Constructing_a_new_event_base_it_should_set_the_event_time_stap_to_the_time_given_by_the_IClock_from_the_NcqrsEnvironment()
        {
            var theTimeStamp = new DateTime(2000, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc);

            var clock = MockRepository.GenerateStrictMock<IClock>();
            clock.Stub(c => c.UtcNow()).Return(theTimeStamp);

            NcqrsEnvironment.SetDefault<IClock>(clock);

            var eventBase = MockRepository.GenerateStub<Event>();
            eventBase.EventTimeStamp.Should().Be(theTimeStamp);
        }
    }
}
