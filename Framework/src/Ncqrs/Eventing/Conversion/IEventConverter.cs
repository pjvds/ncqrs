using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing.Conversion
{
    [ContractClass(typeof(IEventConverterContracts))]
    public interface IEventConverter
    {
        /// <summary>
        /// Converts an event to another event.
        /// </summary>
        /// <param name="eventToConvert">The event to convert.</param>
        /// <returns>A new event based on the <paramref name="eventToConvert"/>. This is a different type that the type of the <see cref="eventToConvert"/>.</returns>
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
            Contract.Ensures(Contract.Result<ISourcedEvent>().GetType() != eventToConvert.GetType());

            return default(ISourcedEvent);
        }
    }
}