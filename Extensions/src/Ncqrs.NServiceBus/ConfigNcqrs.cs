using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.ServiceModel;
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
         _commandService = new NsbCommandService();
         config.Configurer.RegisterSingleton(typeof (ICommandService),
                                             _commandService);          
      }

      public ConfigNcqrs UseMappedExecutors()
      {
         _commandService.UseMappedExecutors = true;
         return this;
      }

      public ConfigNcqrs RegisterExecutor<TCommand>(ICommandExecutor<TCommand> executor) where TCommand : ICommand
      {
         _commandService.RegisterExecutor(executor);
         return this;
      }
   }
}