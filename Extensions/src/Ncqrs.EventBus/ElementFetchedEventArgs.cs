using System;

namespace Ncqrs.EventBus
{
    public class ElementFetchedEventArgs : EventArgs
    {
        private readonly IProcessingElement _processingElement;

        public ElementFetchedEventArgs(IProcessingElement processingElement)
        {
            _processingElement = processingElement;
        }

        public IProcessingElement ProcessingElement
        {
            get { return _processingElement; }
        }
    }
}