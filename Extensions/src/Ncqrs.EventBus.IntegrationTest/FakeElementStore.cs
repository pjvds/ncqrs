using System;
using System.Collections.Generic;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus.IntegrationTest
{
    public class FakeElementStore : IBrowsableElementStore
    {
        private readonly Random _random = new Random();
        private const int Count = 1000;
        private int _fetched;

        public void SetCursorPositionAfter(Guid? lastEventId)
        {
        }

        public IEnumerable<IProcessingElement> Fetch(string pipelineName, int maxCount)
        {
            lock (this)
            {
                int count = _random.Next(maxCount);
                int available = Count - _fetched;
                count = count > available ? available : count;
                for (int i = 0; i < count; i++)
                {
                    _fetched++;
                    yield return new SourcedEventProcessingElement(new RandomEvent(_fetched));
                }
            }
        }

        public void MarkLastProcessedEvent(string pipelineName, IProcessingElement evnt)
        {            
        }
    }
}