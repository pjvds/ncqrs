using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using NServiceBus;

namespace Ncqrs.NServiceBus
{
    public class ConfigNcqrs : Configure
    {
        private NsbCommandService _commandService;
        private InProcessEventBus _inProcessEventBus;

        public void Configure(Configure config)
        {

            Builder = config.Builder;
            Configurer = config.Configurer;

            NcqrsEnvironment.Configure(new NsbEnvironmentConfiguration(Builder));

            NcqrsEnvironment.SetDefault<InProcessEventBus>(new InProcessEventBus(false));
            NcqrsEnvironment.SetDefault<NsbCommandService>(new NsbCommandService());

            var compositeBus = new CompositeEventBus();

            _inProcessEventBus = NcqrsEnvironment.Get<InProcessEventBus>();
            compositeBus.AddBus(_inProcessEventBus);

            NcqrsEnvironment.SetDefault<NsbEventBus>(new NsbEventBus());
            compositeBus.AddBus(NcqrsEnvironment.Get<NsbEventBus>());

            NcqrsEnvironment.SetDefault<IEventBus>(compositeBus);

            _commandService = NcqrsEnvironment.Get<NsbCommandService>();
            config.Configurer.RegisterSingleton(typeof(ICommandService),
                                                _commandService);
        }

        /// <summary>
        /// Registers custom executor in Ncqrs runtime.
        /// </summary>
        /// <typeparam name="TCommand">Type of command which will be affected.</typeparam>
        /// <param name="executor">Custom executor instance.</param>
        /// <returns>Self.</returns>
        public ConfigNcqrs RegisterExecutor<TCommand>(ICommandExecutor<TCommand> executor) where TCommand : ICommand
        {
            if (_commandService == null)
            {
                throw new InvalidOperationException("RegisterExecutor cannot be called before the Command Service has been initialized.  This occurs in Ncqrs.NServiceBus.ConfigNcqrs.Configure operation.");
            }
            _commandService.RegisterExecutor(executor);
            return this;
        }


        /// <summary>
        /// Register a handler that will receive all messages that are published.
        /// </summary>
        /// <param name="handler">The handler to register.</param>
        public ConfigNcqrs RegisterInProcessEventHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
        {
            if (_inProcessEventBus == null)
            {
                throw new InvalidOperationException("RegisterExecutor cannot be called before the InProcess Event Bus has been initialized.  This occurs in Ncqrs.NServiceBus.ConfigNcqrs.Configure operation.");
            }
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
            if (_inProcessEventBus == null)
            {
                throw new InvalidOperationException("RegisterExecutor cannot be called before the InProcess Event Bus has been initialized.  This occurs in Ncqrs.NServiceBus.ConfigNcqrs.Configure operation.");
            }
            _inProcessEventBus.RegisterHandler(eventType, handler);
            return this;
        }

        /// <summary>
        /// Registers custom interceptor in Ncqrs runtime.
        /// </summary>
        /// <param name="interceptor">Custom interceptor instance.</param>
        /// <returns>Self.</returns>
        public ConfigNcqrs RegisterInterceptor(ICommandServiceInterceptor interceptor)
        {
            if (_commandService == null)
            {
                throw new InvalidOperationException("RegisterExecutor cannot be called before the Command Service has been initialized.  This occurs in Ncqrs.NServiceBus.ConfigNcqrs.Configure operation.");
            }
            _commandService.AddInterceptor(interceptor);
            return this;
        }

    }
}