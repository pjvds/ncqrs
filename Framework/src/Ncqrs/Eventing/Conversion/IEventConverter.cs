using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing.Conversion
{
    /// <summary>
    /// Converts an event into another event.
    /// <para>
    /// This can be used to handle changes in events. Instead of changing an
    /// existing event by for example adding a new property to it you should
    /// introduce a new event. When you have done this you should create a
    /// converter that can convert all the old to events to the new introduced
    /// event.
    /// </para>
    /// </summary>
    [ContractClass(typeof(IEventConverterContracts<,>))]
    public interface IEventConverter<TFrom, TTo>
        where TFrom : IEvent
        where TTo : IEvent
    {
        /// <summary>
        /// Converts an event. It return a new transformed event based on the 
        /// <paramref name="eventToConvert"/>.
        /// </summary>
        /// <param name="eventToConvert">The event to convert.</param>
        /// <returns>A new event based on the <paramref name="eventToConvert"/>.
        /// </returns>
        TTo Convert(TFrom eventToConvert);
    }

    [ContractClassFor(typeof(IEventConverter<,>))]
    internal sealed class IEventConverterContracts<TFrom, TTo> : IEventConverter<TFrom, TTo>
        where TFrom : IEvent
        where TTo : IEvent
    {
        public TTo Convert(TFrom eventToConvert)
        {
            Contract.Requires<ArgumentNullException>(eventToConvert != null, "The eventToConvert cannot be null.");
            Contract.Ensures(Contract.Result<TTo>() != null);

            return default(TTo);
        }
    }
}