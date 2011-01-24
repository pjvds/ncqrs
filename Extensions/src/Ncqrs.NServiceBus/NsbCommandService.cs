using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding.ServiceModel;

namespace Ncqrs.NServiceBus
{
    /// <summary>
    /// Ncqrs command service integrated with NServiceBus. Main change affects the way
    /// executors are being resolved. If there is no specific executor associated with
    /// given command type, if the command can be mapped <see cref="MappedCommandExecutor{TCommand}"/> 
    /// is used automatically.
    /// </summary>
    public class NsbCommandService : CommandService
    {
        public new void RegisterExecutor<TCommand>(ICommandExecutor<TCommand> executor) where TCommand : ICommand
        {
            base.RegisterExecutor(executor);
        }
        protected override Action<ICommand> GetCommandExecutorForCommand(Type commandType)
        {
            var registeredExecutor = base.GetCommandExecutorForCommand(commandType);
            if (registeredExecutor == null)
            {
                var factory = new AttributeBasedCommandMapper();
                if (factory.CanMapCommand(commandType))
                {
                    registeredExecutor = GetMappedExecutorAction(commandType);
                }
            }
            return registeredExecutor;
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
            private readonly ICommandExecutor<T> _executor;

            public MappedCommandExecutorProxy()
            {
                var factory = new AttributeBasedCommandMapper();
                _executor = factory.CreateExecutorForCommand<T>();
            }

            public void Execute(ICommand command)
            {
                _executor.Execute((T)command);
            }
        }
    }
}