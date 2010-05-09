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
    [ContractClass(typeof(IEventConverterContracts))]
    public interface IEventConverter
    {
        /// <summary>
        /// Converts an event. It return a new transformed event based on the 
        /// <paramref name="eventToConvert"/>. The 
        /// <see cref="ISourcedEvent.EventSourceId"/>, 
        /// <see cref="ISourcedEvent.EventSequence"/>, 
        /// <see cref="ISourcedEvent.EventIdentifier"/> and the 
        /// <see cref="ISourcedEvent.EventTimeStamp"/> should stay the same.
        /// </summary>
        /// <param name="eventToConvert">The event to convert.</param>
        /// <returns>A new event based on the <paramref name="eventToConvert"/>.
        /// </returns>
        ISourcedEvent Convert(ISourcedEvent eventToConvert);
    }

    [ContractClassFor(typeof(IEventConverter))]
    internal sealed class IEventConverterContracts : IEventConverter
    {
        public ISourcedEvent Convert(ISourcedEvent eventToConvert)
        {
            Contract.Requires<ArgumentNullException>(eventToConvert != null, "The eventToConvert cannot be null.");

            Contract.Ensures(Contract.Result<ISourcedEvent>().EventSourceId == eventToConvert.EventSourceId, "The EventSourceId should not be changed after conversion.");
            Contract.Ensures(Contract.Result<ISourcedEvent>().EventSequence == eventToConvert.EventSequence, "The EventSequence should not be changed after conversion.");
            Contract.Ensures(Contract.Result<ISourcedEvent>().EventIdentifier == eventToConvert.EventIdentifier, "The EventIdentifier should not be changed after conversion.");
            Contract.Ensures(Contract.Result<ISourcedEvent>().EventTimeStamp == eventToConvert.EventTimeStamp, "The EventTimeStamp should not be changed after conversion.");

            return default(ISourcedEvent);
        }
    }
}