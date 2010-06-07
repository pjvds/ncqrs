using System;
using Ncqrs;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Messaging;
using Ncqrs.Messaging.NServiceBus;
using Ncqrs.NServiceBus;
using NServiceBus;
using NServiceBus.ObjectBuilder;

namespace NServiceBus
{
    public class ConfigNcqrs : Configure
    {
        private MessageService _messageService;
        private InProcessEventBus _inProcessEventBus;
        private MessageSendingEventHandler _sendingEventHandler;

        public void Configure(Configure config)
        {
            Builder = config.Builder;
            Configurer = config.Configurer;

            NcqrsEnvironment.Configure(new NsbEnvironmentConfiguration(Builder));
            var compositeBus = new CompositeEventBus();
            _inProcessEventBus = new InProcessEventBus(false);
            compositeBus.AddBus(new NsbEventBus());
            compositeBus.AddBus(_inProcessEventBus);
            _sendingEventHandler = new MessageSendingEventHandler();
            _inProcessEventBus.RegisterHandler(_sendingEventHandler);
            NcqrsEnvironment.SetDefault(compositeBus);
            _messageService = new MessageService();
            config.Configurer.RegisterSingleton(typeof(IMessageService), _messageService);
        }

        public ConfigNcqrs UseReceivingStrategy(Func<object, bool> condition, IReceivingStrategy receivingStrategy)
        {
            _messageService.UseReceivingStrategy(new ConditionalReceivingStrategy(condition, receivingStrategy));
            return this;
        }

        /// <summary>
        /// Register a handler that will receive all messages that are published.
        /// </summary>
        /// <param name="handler">The handler to register.</param>
        public ConfigNcqrs RegisterInProcessEventHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
        {
            _inProcessEventBus.RegisterHandler(handler);
            return this;
        }


        /// <summary>
        /// Register a handler that will receive all messages that are published.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler to register.</param>
        public ConfigNcqrs RegisterInProcessEventHandler(Type eventType, Action<IEvent> handler)
        {
            _inProcessEventBus.RegisterHandler(eventType, handler);
            return this;
        }
    }
}