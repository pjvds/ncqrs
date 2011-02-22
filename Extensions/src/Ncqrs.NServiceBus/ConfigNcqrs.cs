using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using NServiceBus;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

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
            var compositeBus = new CompositeEventBus();
            _inProcessEventBus = new InProcessEventBus(false);
            compositeBus.AddBus(new NsbEventBus());
            compositeBus.AddBus(_inProcessEventBus);
            NcqrsEnvironment.SetDefault<IEventBus>(compositeBus);
            _commandService = new NsbCommandService();
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
            _commandService.RegisterExecutor(executor);
            return this;
        }

        /// <summary>
        /// Registers the executors for mapped commands in assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        public ConfigNcqrs RegisterExecutorsForMappedCommandsInAssembly(System.Reflection.Assembly assembly)
        {
            _commandService.RegisterExecutorsInAssembly(assembly);
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