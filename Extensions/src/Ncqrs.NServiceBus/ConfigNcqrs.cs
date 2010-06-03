using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing.ServiceModel.Bus;
using NServiceBus;
using NServiceBus.ObjectBuilder;

namespace Ncqrs.NServiceBus
{
    public class ConfigNcqrs : Configure
    {
        private NsbCommandService _commandService;

        public void Configure(Configure config)
        {

            Builder = config.Builder;
            Configurer = config.Configurer;

            NcqrsEnvironment.Configure(new NsbEnvironmentConfiguration(Builder));
            IEventBus nsbEventBus = new NsbEventBus();
            NcqrsEnvironment.SetDefault(nsbEventBus);
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
    }
}