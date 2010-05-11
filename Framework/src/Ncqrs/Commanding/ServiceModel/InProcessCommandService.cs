using System;
using Ncqrs.Commanding.CommandExecution;

namespace Ncqrs.Commanding.ServiceModel
{
    /// <summary>
    /// A in process command service.
    /// </summary>
    public class InProcessCommandService : CommandServiceBase
    {
        public void RegisterExecutor<TCommand>(ICommandExecutor<TCommand> executor) where TCommand : ICommand
        {
            base.RegisterExecutor(executor);
        }

        public void UnregisterExecutor<TCommand>(ICommandExecutor<TCommand> executor) where TCommand : ICommand
        {
            base.UnregisterExecutor<TCommand>();
        }

        public void AddInterceptor(ICommandServiceInterceptor interceptor)
        {
            base.AddInterceptor(interceptor);
        }

        public void RemoveInterceptor(ICommandServiceInterceptor interceptor)
        {
            base.RemoveInterceptor(interceptor);
        }
    }
}