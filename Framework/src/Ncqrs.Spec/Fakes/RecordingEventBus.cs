using System.Collections.Generic;
using System.Linq;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Spec.Fakes
{
    public class RecordingEventBus : IEventBus 
    {
        private readonly IEventBus _realBus;
        private readonly List<IPublishableEvent> _recording;

        public RecordingEventBus()
            : this(NcqrsEnvironment.Get<IEventBus>())
        {
        }

        public RecordingEventBus(IEventBus realBus)
        {
            _realBus = realBus;
            _recording = new List<IPublishableEvent>();
        }

        public void Publish(IPublishableEvent eventMessage)
        {
            _recording.Add(eventMessage);
            _realBus.Publish(eventMessage);
        }

        public void Publish(IEnumerable<IPublishableEvent> eventMessages)
        {
            var messages = eventMessages.ToArray();
            _recording.AddRange(messages);
            _realBus.Publish(messages);
        }

        public IEnumerable<IPublishableEvent> GetPublishedEvents()
        {
            return _recording.ToArray();
        }


    }
}
