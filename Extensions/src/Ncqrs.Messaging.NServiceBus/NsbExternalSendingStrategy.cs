using System;
using System.Threading;
using NServiceBus;

namespace Ncqrs.Messaging.NServiceBus
{    
    public class NsbExternalSendingStrategy<TIn, TOut> : ISendingStrategy 
        where TOut : IMessage
    {
        private readonly Func<OutgoingMessage, TIn, Action<TOut>> _mappingFunction;        

        public NsbExternalSendingStrategy(Func<OutgoingMessage, TIn, Action<TOut>> mappingFunction)
        {
            _mappingFunction = mappingFunction;
        }


        public void Send(OutgoingMessage message)
        {
            Bus.Send(_mappingFunction(message, (TIn) message.Payload));
        }

        private static IBus Bus
        {
            get { return NcqrsEnvironment.Get<IBus>(); }
        }
    }
}