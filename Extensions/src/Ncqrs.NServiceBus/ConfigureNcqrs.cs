using Ncqrs;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.NServiceBus;

namespace NServiceBus
{
    public static class ConfigureNcqrs
    {
        /// <summary>
        /// Instructs NServiceBus to install Ncqrs message handler. All <see cref="CommandMessage"/>s
        /// will be routed to Ncqrs where they will be handled.
        /// 
        /// By default, all commands will be executed using <see cref="MappedCommandExecutor{TCommand}"/>.
        /// To customize this, you can use <see cref="ConfigNcqrs.RegisterExecutor{TCommand}"/>.
        /// </summary>
        /// <param name="config">The config object.</param>
        /// <returns></returns>
        public static ConfigNcqrs InstallNcqrs(this Configure config)
        {
            var configNcqrs = new ConfigNcqrs();
            configNcqrs.Configure(config);
            return configNcqrs;
        }
    }
}