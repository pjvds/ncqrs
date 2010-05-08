using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing.Convertion
{
    public class DelegateBasedConverter<TIn, TOut> : IEventConverter
        where TIn : ISourcedEvent
        where TOut : ISourcedEvent
    {
        private readonly Converter<TIn, TOut> _converter;

        public DelegateBasedConverter(Converter<TIn, TOut> converter)
        {
            Contract.Requires<ArgumentNullException>(converter != null, "The converter cannot be null.");

            _converter = converter;
        }

        public ISourcedEvent Convert(ISourcedEvent eventToConvert)
        {
            TIn input = (TIn)eventToConvert;
            TOut output = _converter(input);

            return output;
        }
    }
}
