using System;
using System.Collections.Generic;

namespace Ncqrs.EventBus
{
    public class CursorPositionCalculator
    {
        private int _lastEventInSequence;
        private int _count;
        private int _sequenceLength;
        private readonly SortedSet<int> _eventsNotInSequence = new SortedSet<int>();

        public CursorPositionCalculator(int startingSequenceNumber)
        {
            _lastEventInSequence = startingSequenceNumber;
        }

        public void Append(SequencedEvent sequencedEvent)
        {
            _count++;    
            if (sequencedEvent.Sequence == _lastEventInSequence + 1)
            {
                _lastEventInSequence++;
                _sequenceLength++;
                ProcessEventsNotInSequence();
            }
            else
            {
                _eventsNotInSequence.Add(sequencedEvent.Sequence);
            }
        }

        private void ProcessEventsNotInSequence()
        {
            var enumerator = _eventsNotInSequence.GetEnumerator();
            while (enumerator.MoveNext() && enumerator.Current == _lastEventInSequence + 1)
            {
                _lastEventInSequence++;
                _sequenceLength++;
            }            
            _eventsNotInSequence.RemoveWhere(x => x <= _lastEventInSequence);
        }

        public int Count
        {
            get {return _count;}
        }

        public int SequenceLength
        {
            get { return _sequenceLength;}
        }        
    }
}