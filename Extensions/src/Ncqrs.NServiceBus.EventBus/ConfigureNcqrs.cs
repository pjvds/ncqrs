using Ncqrs;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.NServiceBus;
using Ncqrs.NServiceBus.EventBus;

namespace NServiceBus
{
    public static class ConfigureNcqrs
    {
        /// <summary>
        /// Instructs NServiceBus to install Ncqrs message handler. All <see cref="CommandMessage"/>s
        /// will be routed to Ncqrs where they will be handled.
        /// 
        /// By default, all commands will be executed using <see cref="MappedCommandExecutor{TCommand}"/>.
        /// To customize this, you can use <see cref="EventBusConfigNcqrs.RegisterExecutor{TCommand}"/>.
        /// </summary>
        /// <param name="config">The config object.</param>
        /// <returns></returns>
        public static EventBusConfigNcqrs InstallNcqrsWithEventBus(this Configure config)
        {
            var configNcqrs = new EventBusConfigNcqrs();
            configNcqrs.Configure(config);
            return configNcqrs;
        }
    }

    
}