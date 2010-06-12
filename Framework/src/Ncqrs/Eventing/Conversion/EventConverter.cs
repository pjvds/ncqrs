using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Conversion
{
    /// <summary>
    /// A dispatcher that dispatch event conversion to the converters that have been added based on event type.
    /// When an event should be converted it resolved the corresponding converter and uses that one to convert the event. When there
    /// is also a converter that can convert the result of that conversion that one is called.
    /// </summary>
    public class EventConverter : IEventConverter<IEvent, IEvent>
    {
        private Dictionary<Type, Converter<IEvent, IEvent>> _converters = new Dictionary<Type, Converter<IEvent, IEvent>>();

        /// <summary>
        /// Adds the converter for a specific event type.
        /// </summary>
        /// <typeparam name="TFrom">The type of the event that will be converted.</typeparam>
        /// <typeparam name="TTo">The type of the result of the conversion.</typeparam>
        /// <param name="converter">The converter method that does the conversion.</param>
        /// <returns>The current <see cref="EventConverter"/> that can be used to chain method calls.</returns>
        public EventConverter AddConverter<TFrom, TTo>(Converter<TFrom, TTo> converter) where TFrom : IEvent where TTo : IEvent
        {
            Contract.Requires<ArgumentNullException>(converter != null, "The converter cannot be null.");

            var converterToAdd = new DelegateBasedConverter<TFrom, TTo>(converter);
            return AddConverter(converterToAdd);
        }

        /// <summary>
        /// Adds the converter for a specific event type.
        /// </summary>
        /// <param name="eventSourceType">The type of the event that will be converted.</param>
        /// <param name="converter">The converter method that does the conversion.</param>
        /// <returns>The current <see cref="EventConverter"/> that can be used to chain method calls.</returns>
        public EventConverter AddConverter<TFrom, TTo>(IEventConverter<TFrom, TTo> converter) where TFrom : IEvent where TTo : IEvent
        {
            Contract.Requires<ArgumentNullException>(converter != null, "The converter cannot be null.");

            _converters.Add(typeof(TFrom), (x) => converter.Convert((TFrom)x));
            return this;
        }

        public EventConverter AddConverters(Assembly assembly)
        {
            var converterTypeQuery = from t in assembly.GetTypes()
                                     let convercionInterfaces = from i in t.GetInterfaces()
                                                                where
                                                                    i.IsGenericType &&
                                                                    i.GetGenericTypeDefinition() == typeof(IEventConverter<,>)
                                                                select i
                                     where convercionInterfaces.Count() > 0
                                     select new { Type = t, ConversionInterfaces = convercionInterfaces };

            foreach(var c in converterTypeQuery)
            {
                var converter = Activator.CreateInstance(c.Type, Type.EmptyTypes);

                foreach (var ci in c.ConversionInterfaces)
                {
                    var convertMethod = ci.GetMethod("Convert");
                    var fromType = ci.GetGenericArguments().First();

                    Converter<IEvent, IEvent> convertClosure =
                        (x => (IEvent) convertMethod.Invoke(converter, new object[] {x}));
                    _converters.Add(fromType, convertClosure);
                }
            }

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
        public IEvent Convert(IEvent eventToConvert)
        {
            Type eventType = eventToConvert.GetType();
            var convertedEvent = eventToConvert;

            Converter<IEvent, IEvent> converter = null;

            // If we have a converter, convert it.
            if(_converters.TryGetValue(eventType, out converter))
            {
                // Convert the event.
                var e = converter(eventToConvert);

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
