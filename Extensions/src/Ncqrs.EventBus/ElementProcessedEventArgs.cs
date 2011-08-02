using System;

namespace Ncqrs.EventBus
{
    public class ElementProcessedEventArgs : EventArgs
    {
        private readonly IProcessingElement _processedElement;

        public ElementProcessedEventArgs(IProcessingElement processedElement)
        {
            _processedElement = processedElement;
        }

        public IProcessingElement ProcessedElement
        {
            get { return _processedElement; }
        }
    }
}