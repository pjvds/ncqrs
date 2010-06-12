using System;
using System.Diagnostics.Contracts;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Conversion
{
    /// <summary>
    /// A converter that uses a <see cref="Converter{TFrom, TTo}"/> delegate to execute the actual conversion.
    /// </summary>
    /// <typeparam name="TFrom">The type of the event that will be converted.</typeparam>
    /// <typeparam name="TTo">The type of the result of the conversion.</typeparam>
    public class DelegateBasedConverter<TFrom, TTo> : IEventConverter<TFrom, TTo>
        where TFrom : IEventData
        where TTo : IEventData
    {
        private readonly Converter<TFrom, TTo> _converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateBasedConverter&lt;TFrom, TTo&gt;"/> class.
        /// </summary>
        /// <param name="converter">The converter that will do the actual conversion.</param>
        public DelegateBasedConverter(Converter<TFrom, TTo> converter)
        {
            Contract.Requires<ArgumentNullException>(converter != null, "The converter cannot be null.");

            _converter = converter;
        }

        /// <summary>
        /// Converts an event. It return a new transformed event based on the
        /// <paramref name="eventToConvert"/>. The
        /// </summary>
        /// <param name="eventToConvert">The event to convert.</param>
        /// <returns>
        /// A new event based on the <paramref name="eventToConvert"/>.
        /// </returns>
        public TTo Convert(TFrom eventToConvert)
        {
            return _converter(eventToConvert);
        }
    }
}
