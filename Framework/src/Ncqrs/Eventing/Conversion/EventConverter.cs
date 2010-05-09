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

        /// <summary>
        /// Adds the converter for a specific event type.
        /// </summary>
        /// <typeparam name="TFrom">The type of the event that will be converted.</typeparam>
        /// <typeparam name="TTo">The type of the result of the conversion.</typeparam>
        /// <param name="converter">The converter method that does the conversion.</param>
        /// <returns>The current <see cref="EventConverter"/> that can be used to chain method calls.</returns>
        public EventConverter AddConverter<TFrom, TTo>(Converter<TFrom, TTo> converter) where TFrom : ISourcedEvent where TTo : ISourcedEvent
        {
            Contract.Requires<ArgumentNullException>(converter != null, "The converter cannot be null.");

            var converterToAdd = new DelegateBasedConverter<TFrom, TTo>(converter);
            return AddConverter(typeof (TFrom), converterToAdd);
        }

        /// <summary>
        /// Adds the converter for a specific event type.
        /// </summary>
        /// <param name="eventSourceType">The type of the event that will be converted.</param>
        /// <param name="converter">The converter method that does the conversion.</param>
        /// <returns>The current <see cref="EventConverter"/> that can be used to chain method calls.</returns>
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

            // If we have a converter, convert it.
            if(_converters.TryGetValue(eventType, out converter))
            {
                // Convert the event.
                var e = converter.Convert(eventToConvert);

                // When the result of the convertion has a
                // different type that the source, try
                // to convert the result to.
                // Otherwise, we assume that the event was
                // not converted. So it doesn't need another
                // conversion.
                if (e.GetType() != eventToConvert.GetType())
                {
                    convertedEvent = Convert(e);
                }
            }

            return convertedEvent;
        }
    }
}
