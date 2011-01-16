using System.Collections.Generic;

namespace Ncqrs.EventBus
{
    public class FixedSizeChunkBuffer
    {
        private static readonly IProcessingElement[] _emptyBatch = new IProcessingElement[] {};
        private readonly int _chunkSize;
        private readonly LinkedList<List<IProcessingElement>> _storage = new LinkedList<List<IProcessingElement>>();

        public FixedSizeChunkBuffer(int chunkSize)
        {
            _chunkSize = chunkSize;
            _storage.AddFirst(new List<IProcessingElement>());
        }

        public void Append(IProcessingElement element)
        {
            var tail = _storage.Last.Value;
            tail.Add(element);
            if (tail.Count == _chunkSize)
            {
                _storage.AddLast(new List<IProcessingElement>());
            }
        }

        public IEnumerable<IProcessingElement> TakeOne()
        {
            var head = _storage.First.Value;
            if (head.Count == 0)
            {
                return _emptyBatch;
            }
            _storage.RemoveFirst();
            if (_storage.Count == 0)
            {
                _storage.AddFirst(new List<IProcessingElement>());
            }
            return head;
        }
    }
}