using NUnit.Framework;
using FluentAssertions;

namespace Ncqrs.EventBus.Tests
{
    [TestFixture]
    public class CursorPositionCalculatorSpecification
    {
        [Test]
        public void When_event_does_not_lengthen_the_sequence()
        {
            var sut = new CursorPositionCalculator(0);
            sut.Append(new SourcedEventProcessingElement(2, new TestEvent()));

            sut.Count.Should().Be(1);
            sut.SequenceLength.Should().Be(0);
        }        

        [Test]
        public void When_event_lengthens_the_sequence()
        {
            var sut = new CursorPositionCalculator(0);
            sut.Append(new SourcedEventProcessingElement(1, new TestEvent()));

            sut.Count.Should().Be(1);
            sut.SequenceLength.Should().Be(1);   
        }
        
        [Test]
        public void When_event_fills_gap_in_sequence_sequence_length_is_incremented_by_gap_size()
        {
            var sut = new CursorPositionCalculator(0);
            sut.Append(new SourcedEventProcessingElement(2, new TestEvent()));
            sut.Append(new SourcedEventProcessingElement(3, new TestEvent()));
            sut.Append(new SourcedEventProcessingElement(5, new TestEvent()));

            sut.Append(new SourcedEventProcessingElement(1, new TestEvent()));

            sut.Count.Should().Be(4);
            sut.SequenceLength.Should().Be(3);            
        } 

        [Test]
        public void When_clearing_sequence()
        {
            var sut = new CursorPositionCalculator(0);
            sut.Append(new SourcedEventProcessingElement(1, new TestEvent()));
            sut.Append(new SourcedEventProcessingElement(2, new TestEvent()));
            sut.Append(new SourcedEventProcessingElement(4, new TestEvent()));

            sut.ClearSequence();

            sut.Count.Should().Be(1);
            sut.SequenceLength.Should().Be(0);            
        }
    }
}