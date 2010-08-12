using System;
using System.Collections.Generic;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus.IntegrationTest
{
    public class FakeEventStore : IEventStore
    {
        private readonly Random _random = new Random();

        public void SetCursorPositionAfter(Guid lastEventId)
        {            
        }

        public IEnumerable<SourcedEvent> FetchEvents(int maxCount)
        {
            int count = _random.Next(maxCount);
            for (int i = 0; i < count; i++)
            {
                yield return new RandomEvent();
            }
        }
    }
}