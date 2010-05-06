using System;
using Ncqrs.Commanding.CommandExecution;

namespace Ncqrs.Commanding.ServiceModel
{
    /// <summary>
    /// A in process command service.
    /// </summary>
    public class InProcessCommandService : CommandServiceBase
    {
        public void RegisterExecutor<TCommand>(ICommandExecutor executor) where TCommand : ICommand
        {
            base.RegisterExecutor<TCommand>(executor);
        }

        public void RegisterExecutor(Type commandType, ICommandExecutor executor)
        {
            base.RegisterExecutor(commandType, executor);
        }

        public void UnregisterExecutor(Type commandType, ICommandExecutor executor)
        {
            base.UnregisterExecutor(commandType, executor);
        }
    }
}