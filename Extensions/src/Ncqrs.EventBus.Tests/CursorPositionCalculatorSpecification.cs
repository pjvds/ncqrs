using NUnit.Framework;
using FluentAssertions;

namespace Ncqrs.EventBus.Tests
{
    [TestFixture]
    public class CursorPositionCalculatorSpecification
    {
        [Test]
        public void When_event_does_not_lengthen_the_sequence_event_counter_is_incremented()
        {
            var sut = new CursorPositionCalculator(0);
            sut.Append(new SequencedEvent(2, new TestEvent()));

            sut.Count.Should().Be(1);
        }

        [Test]
        public void When_event_does_not_lengthen_the_sequence_sequence_length_is_not_incremented()
        {
            var sut = new CursorPositionCalculator(0);
            sut.Append(new SequencedEvent(2, new TestEvent()));

            sut.SequenceLength.Should().Be(0);
        }

        [Test]
        public void When_event_lengthens_the_sequence_event_counter_is_incremented()
        {
            var sut = new CursorPositionCalculator(0);
            sut.Append(new SequencedEvent(1, new TestEvent()));

            sut.Count.Should().Be(1);
        }

        [Test]
        public void When_event_lengthens_the_sequence_sequence_length_is_incremented()
        {
            var sut = new CursorPositionCalculator(0);
            sut.Append(new SequencedEvent(1, new TestEvent()));

            sut.SequenceLength.Should().Be(1);            
        }

        [Test]
        public void When_event_fills_gap_in_sequence_sequence_length_is_incremented_by_gap_size()
        {
            var sut = new CursorPositionCalculator(0);
            sut.Append(new SequencedEvent(2, new TestEvent()));
            sut.Append(new SequencedEvent(3, new TestEvent()));
            sut.Append(new SequencedEvent(5, new TestEvent()));

            sut.Append(new SequencedEvent(1, new TestEvent()));

            sut.SequenceLength.Should().Be(3);
        } 
    }
}