using System;
using System.Diagnostics;
using System.Threading;

namespace Ncqrs.EventBus
{    
    public class PipelineProcessor
    {
        private readonly IElementProcessor _elementProcessor;

        public PipelineProcessor(
            IElementProcessor elementProcessor)
        {
            _elementProcessor = elementProcessor;
        }

        public event EventHandler<ElementProcessedEventArgs> EventProcessed;

        private void OnEventProcessed(ElementProcessedEventArgs e)
        {
            var handler = EventProcessed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void ProcessNext(IProcessingElement element)
        {
            try
            {
                _elementProcessor.Process(element);
                OnEventProcessed(new ElementProcessedEventArgs(element));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception during ProcessedElement processing: "+ex);
            }            
        }
    }
}