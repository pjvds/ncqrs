using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing.Conversion
{
    /// <summary>
    /// A dispatcher that dispatch event conversion to the converters that have been added based on event type.
    /// When an event should be converted it resolved the corresponding converter and uses that one to convert the event. When there
    /// is also a converter that can convert the result of that conversion that one is called.
    /// </summary>
    public class EventConverter : IEventConverter
    {
        private Dictionary<Type, IEventConverter> _converters = new Dictionary<Type, IEventConverter>();

        public EventConverter AddConverter<TFrom, TTo>(Converter<TFrom, TTo> converter) where TFrom : ISourcedEvent where TTo : ISourcedEvent
        {
            Contract.Requires<ArgumentNullException>(converter != null, "The converter cannot be null.");

            var converterToAdd = new DelegateBasedConverter<TFrom, TTo>(converter);
            return AddConverter(typeof (TFrom), converterToAdd);
        }

        public EventConverter AddConverter(Type eventSourceType, IEventConverter converter)
        {
            Contract.Requires<ArgumentNullException>(eventSourceType != null, "The eventSourceType cannot be null.");
            Contract.Requires<ArgumentNullException>(converter != null, "The converter cannot be null.");

            _converters.Add(eventSourceType, converter);
            return this;
        }

        /// <summary>
        /// Converts an event. It uses all converters available to convert the given event. That means that when an event of Evnt_V1 is given and there are converters
        /// that can convert Evnt_V1 intro Evnt_V2 and Evnt_V2 in Evnt_V3, the result of this method is a conversion from Evnt_V1 to Evnt_V3.
        /// </summary>
        /// <param name="eventToConvert">The event to convert.</param>
        /// <returns>
        /// A new event based on the <paramref name="eventToConvert"/>.
        /// </returns>
        public ISourcedEvent Convert(ISourcedEvent eventToConvert)
        {
            Type eventType = eventToConvert.GetType();
            var convertedEvent = eventToConvert;

            IEventConverter converter = null;

            if(_converters.TryGetValue(eventType, out converter))
            {
                var e = converter.Convert(eventToConvert);

                if (e.GetType() != eventToConvert.GetType())
                {
                    convertedEvent = Convert(e);
                }
            }

            return convertedEvent;
        }
    }
}
