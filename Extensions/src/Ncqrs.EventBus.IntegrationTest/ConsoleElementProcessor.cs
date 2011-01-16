using System;
using System.Threading;

namespace Ncqrs.EventBus.IntegrationTest
{
    public class ConsoleElementProcessor : IElementProcessor
    {
        private readonly string _pipelinePrefix;
        private readonly int _waitTime;
        public int ProcessedEvents;

        public ConsoleElementProcessor(string pipelinePrefix, int waitTime)
        {
            _pipelinePrefix = pipelinePrefix;
            _waitTime = waitTime;
        }

        public void Process(IProcessingElement evnt)
        {
            Thread.Sleep(_waitTime);

            Interlocked.Increment(ref ProcessedEvents);

            var typedElement = (SourcedEventProcessingElement) evnt;

            Console.WriteLine("{0}: Processing event {1} (id {2})", _pipelinePrefix, typedElement.Event.EventSequence, evnt.UniqueId);
        }
    }
}