using System;
using System.Diagnostics;
using System.Threading;

namespace Ncqrs.EventBus
{    
    public class PipelineProcessor
    {
        private readonly IEventProcessor _eventProcessor;

        public PipelineProcessor(
            IEventProcessor eventProcessor)
        {
            _eventProcessor = eventProcessor;
        }

        public event EventHandler<EventProcessedEventArgs> EventProcessed;

        private void OnEventProcessed(EventProcessedEventArgs e)
        {
            var handler = EventProcessed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void ProcessNext(SequencedEvent evnt)
        {
            try
            {
                _eventProcessor.Process(evnt.Event);
                OnEventProcessed(new EventProcessedEventArgs(evnt));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception during event processing: "+ex);
            }            
        }
    }
}