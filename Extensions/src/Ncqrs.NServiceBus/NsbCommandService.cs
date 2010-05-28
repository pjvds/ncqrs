using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.Commanding.ServiceModel;

namespace Ncqrs.NServiceBus
{
   public class NsbCommandService : CommandServiceBase
   {
      public new void RegisterExecutor<TCommand>(ICommandExecutor<TCommand> executor) where TCommand : ICommand
      {
         base.RegisterExecutor(executor);
      }

      public bool UseMappedExecutors { get; set; }

      protected override Action<ICommand> GetCommandExecutorForCommand(Type commandType)
      {
         if (UseMappedExecutors)
         {
            return GetMappedExecutorAction(commandType);
         }
         return base.GetCommandExecutorForCommand(commandType);
      }

      private static Action<ICommand> GetMappedExecutorAction(Type commandType)
      {
         Type executorProxyType =
            typeof(MappedCommandExecutorProxy<>).MakeGenericType(commandType);
         var proxy = (IMappedCommandExecutorProxy)Activator.CreateInstance(executorProxyType);
         return proxy.Execute;
      }

      private interface IMappedCommandExecutorProxy
      {
         void Execute(ICommand command);
      }

      private class MappedCommandExecutorProxy<T> : IMappedCommandExecutorProxy
         where T : ICommand
      {
         private static readonly MappedCommandExecutor<T> _executor = new MappedCommandExecutor<T>();

         public void Execute(ICommand command)
         {
            _executor.Execute((T)command);
         }
      }
   }
}