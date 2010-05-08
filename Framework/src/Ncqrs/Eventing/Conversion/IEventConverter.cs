using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing.Conversion
{
    [ContractClass(typeof(IEventConverterContracts))]
    public interface IEventConverter
    {
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

            return default(ISourcedEvent);
        }
    }
}