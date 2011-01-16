using System;
using System.Linq;
using System.Collections.Generic;

namespace Ncqrs.EventBus
{
    public class CursorPositionCalculator
    {
        private int _lastEventInSequence;
        private string _lastElementtInSequenceId;
        private int _count;
        private int _sequenceLength;
        private readonly SortedDictionary<int, string> _elementsNotInSequence = new SortedDictionary<int, string>();

        public CursorPositionCalculator(int startingSequenceNumber)
        {
            _lastEventInSequence = startingSequenceNumber;
        }

        public void Append(IProcessingElement processingElement)
        {
            _count++;
            if (processingElement.SequenceNumber == _lastEventInSequence + 1)
            {
                _lastEventInSequence++;
                _sequenceLength++;
                _lastElementtInSequenceId = processingElement.UniqueId;
                ProcessEventsNotInSequence();
            }
            else
            {
                _elementsNotInSequence.Add(processingElement.SequenceNumber, processingElement.UniqueId);
            }
        }

        private void ProcessEventsNotInSequence()
        {
            KeyValuePair<int, string> current;
            while (_elementsNotInSequence.Count > 0 && (current = _elementsNotInSequence.First()).Key == GetNextInSequence())
            {
                _lastEventInSequence++;
                _lastElementtInSequenceId = current.Value;
                _sequenceLength++;
                _elementsNotInSequence.Remove(current.Key);
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
     
        public string LastElementtInSequenceId
        {
            get { return _lastElementtInSequenceId; }
        }

        public void ClearSequence()
        {
            _count -= _sequenceLength;
            _sequenceLength = 0;
        }
    }
}