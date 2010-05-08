using System;
using System.Collections.Generic;

namespace Ncqrs.Eventing.Conversion
{
    public class EventConverter : IEventConverter
    {
        private Dictionary<Type, IEventConverter> _converters = new Dictionary<Type, IEventConverter>();

        public EventConverter AddConverter<TIn, TOut>(Converter<TIn, TOut> converter) where TIn : ISourcedEvent where TOut : ISourcedEvent
        {
            var converterToAdd = new DelegateBasedConverter<TIn, TOut>(converter);
            return AddConverter(typeof (TIn), converterToAdd);
        }

        public EventConverter AddConverter(Type eventSourceType, IEventConverter converter)
        {
            _converters.Add(eventSourceType, converter);
            return this;
        }

        public ISourcedEvent Convert(ISourcedEvent eventToConvert)
        {
            Type eventType = eventToConvert.GetType();
            var convertedEvent = eventToConvert;

            IEventConverter converter = null;

            if(_converters.TryGetValue(eventType, out converter))
            {
                convertedEvent = converter.Convert(eventToConvert);    
            }

            return convertedEvent;
        }
    }
}
