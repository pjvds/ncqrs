using System;

namespace Ncqrs.EventBus
{
    public class ElementDemultiplexedEventArgs : EventArgs
    {
        private readonly IProcessingElement _demultiplexedElement;

        public ElementDemultiplexedEventArgs(IProcessingElement demultiplexedElement)
        {
            _demultiplexedElement = demultiplexedElement;
        }

        public IProcessingElement DemultiplexedElement
        {
            get { return _demultiplexedElement; }
        }
    }
}