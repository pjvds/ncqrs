using System;
using System.Linq;
using System.Collections.Generic;

namespace Ncqrs.EventBus
{
    public class CursorPositionCalculator
    {
        private int _lastEventInSequence;
        private Guid _lastEventInSequenceId;
        private int _count;
        private int _sequenceLength;
        private readonly SortedDictionary<int, Guid> _eventsNotInSequence = new SortedDictionary<int, Guid>();

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
               _lastEventInSequenceId = sequencedEvent.Event.EventIdentifier;
                ProcessEventsNotInSequence();
            }
            else
            {
                _eventsNotInSequence.Add(sequencedEvent.Sequence, sequencedEvent.Event.EventIdentifier);
            }
        }

        private void ProcessEventsNotInSequence()
        {
            KeyValuePair<int, Guid> current;
            while (_eventsNotInSequence.Count > 0 && (current = _eventsNotInSequence.First()).Key == GetNextInSequence())
            {
                _lastEventInSequence++;
                _lastEventInSequenceId = current.Value;
                _sequenceLength++;
                _eventsNotInSequence.Remove(current.Key);
            }            
        }

        private int GetNextInSequence()
        {
            return _lastEventInSequence + 1;
        }

        public int Count
        {
            get {return _count;}
        }

        public int SequenceLength
        {
            get { return _sequenceLength;}
        }   
     
        public Guid LastEventInSequenceId
        {
            get { return _lastEventInSequenceId; }
        }

        public void ClearSequence()
        {
            _count -= _sequenceLength;
            _sequenceLength = 0;
        }
    }
}